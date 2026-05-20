using UniDesk.Web.Services;
using UniDesk.Web.Exceptions;
using UniDesk.Web.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UniDesk.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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
    options.UseSqlite(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information);
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<UniDeskDbContext>();
        dbContext.Database.EnsureCreated();

        if (!dbContext.Tickets.Any())
        {
            dbContext.Tickets.AddRange(
                new UniDesk.Web.Models.Ticket
                {
                    Title = "Pierwsze zgloszenie",
                    Description = "Przykladowe zgloszenie startowe",
                    Status = UniDesk.Web.Models.TicketStatus.New
                },
                new UniDesk.Web.Models.Ticket
                {
                    Title = "Drugie zgloszenie",
                    Description = "Zgloszenie w toku",
                    Status = UniDesk.Web.Models.TicketStatus.InProgress
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

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseExceptionHandler();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (EntityNotFoundException ex)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Entity not found",
            Detail = ex.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }
});

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapTicketEndpoints();

app.Run();

public partial class Program
{
}
