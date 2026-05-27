using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using UniDesk.Web;
using UniDesk.Web.Endpoints;
using UniDesk.Web.Exceptions;
using UniDesk.Web.Models;
using UniDesk.Web.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        formatter: new JsonFormatter(renderMessage: true),
        path: "Logs/unidesk-log-.json",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Ciag polaczenia nie moze byc pusty.");
}

var connectionBuilder = new SqliteConnectionStringBuilder(connectionString);
if (!Path.IsPathRooted(connectionBuilder.DataSource))
{
    var databaseDirectory = Path.Combine(
        Path.GetTempPath(),
        "UniDesk");

    Directory.CreateDirectory(databaseDirectory);
    connectionBuilder.DataSource = Path.Combine(databaseDirectory, connectionBuilder.DataSource);
    connectionString = connectionBuilder.ToString();
}

builder.Services.AddDbContext<UniDeskDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services.AddHealthChecks()
    .AddCheck(
        name: "application",
        check: () => HealthCheckResult.Healthy("Application is running"),
        tags: new[] { "live", "ready" })
    .AddDbContextCheck<UniDeskDbContext>(
        name: "database",
        tags: new[] { "ready" });

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<UniDeskDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = ".AspNetCore.Identity.Application";

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TopUniEmailOnly", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireAssertion(context =>
        {
            var email = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                        ?? context.User.Identity?.Name;

            return email != null && email.EndsWith("@top-uni.edu.pl", StringComparison.OrdinalIgnoreCase);
        });
    });
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<UniDeskDbContext>();

        dbContext.Database.Migrate();

        await IdentitySeedData.SeedAsync(scope.ServiceProvider);

        if (!dbContext.Tickets.Any())
        {
            dbContext.Tickets.AddRange(
                new Ticket
                {
                    Title = "Pierwsze zgloszenie",
                    Description = "Przykladowe zgloszenie startowe",
                    Status = TicketStatus.New
                },
                new Ticket
                {
                    Title = "Drugie zgloszenie",
                    Description = "Zgloszenie w toku",
                    Status = TicketStatus.InProgress
                });

            dbContext.SaveChanges();
        }
    }
}

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        return Task.CompletedTask;
    });

    await next();
});

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (EntityNotFoundException ex)
    {
        Log.Warning(
            ex,
            "Entity not found during request {Method} {Path}",
            context.Request.Method,
            context.Request.Path.Value);

        if (!context.Response.HasStarted)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Entity not found",
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
    catch (Exception ex)
    {
        Log.Error(
            ex,
            "Unhandled exception during request {Method} {Path}",
            context.Request.Method,
            context.Request.Path.Value);

        if (!context.Response.HasStarted)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal server error",
                Detail = "Wystapil nieoczekiwany blad aplikacji.",
                Instance = context.Request.Path
            };

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
});

app.UseSerilogRequestLogging();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = WriteHealthResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
});

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapTicketEndpoints();

app.Run();

Log.CloseAndFlush();

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var response = new
    {
        status = report.Status.ToString(),
        totalDuration = report.TotalDuration.ToString(),
        checks = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            description = entry.Value.Description,
            duration = entry.Value.Duration.ToString()
        })
    };

    return context.Response.WriteAsJsonAsync(response);
}

public partial class Program
{
}