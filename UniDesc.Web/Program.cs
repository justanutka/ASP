using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using UniDesc.Web;
using UniDesc.Web.DTOs;
using UniDesc.Web.Models;
using UniDesc.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string cannot be empty.");
}

builder.Services.AddDbContext<UniDeskDbContext>(options =>
    options.UseSqlite(connectionString)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

var app = builder.Build();

app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

var ticketsV2 = app.MapGroup("/api/v2/tickets")
    .WithTags("Tickets V2");

ticketsV2.MapGet("/", (ITicketService ticketService) =>
{
    return Results.Ok(ticketService.GetTicketSummaries());
});

ticketsV2.MapPost("/", (CreateTicketRequest request, ITicketService ticketService) =>
{
    var validationErrors = ValidateRequest(request);
    if (validationErrors != null)
    {
        return Results.ValidationProblem(validationErrors);
    }

    try
    {
        var createdTicket = ticketService.CreateTicket(request);
        return Results.Created($"/api/v2/tickets/{createdTicket.Id}", createdTicket);
    }
    catch (ArgumentException ex)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.Status)] = new[] { ex.Message }
        });
    }
});

ticketsV2.MapDelete("/{id:int}", (int id, ITicketService ticketService) =>
{
    return ticketService.DeleteTicket(id)
        ? Results.NoContent()
        : Results.NotFound();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static Dictionary<string, string[]>? ValidateRequest<TRequest>(TRequest request)
{
    var validationContext = new ValidationContext(request!);
    var validationResults = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(
        request!,
        validationContext,
        validationResults,
        validateAllProperties: true);

    if (isValid)
    {
        return null;
    }

    return validationResults
        .SelectMany(result =>
        {
            var members = result.MemberNames.Any()
                ? result.MemberNames
                : new[] { string.Empty };

            return members.Select(member => new
            {
                Member = member,
                Error = result.ErrorMessage ?? "Validation error."
            });
        })
        .GroupBy(item => item.Member)
        .ToDictionary(
            group => group.Key,
            group => group.Select(item => item.Error).ToArray());
}
