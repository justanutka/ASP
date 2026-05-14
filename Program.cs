using UniDesc.Web.Services;
using UniDesc.Web.Models;
using UniDesc.Web.DTOs;
using UniDesc.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniDesc.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Ciąg połączenia nie może być pusty.");
}

builder.Services.AddDbContext<UniDeskDbContext>(options =>
    options.UseSqlite(connectionString)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

var app = builder.Build();

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

var ticketsApi = app.MapGroup("/api/v2/tickets")
    .WithTags("Tickets v2");

ticketsApi.MapGet("/", (ITicketService ticketService) =>
{
    var tickets = ticketService.GetAllTicketDtos();

    return Results.Ok(tickets);
})
.WithName("GetV2Tickets")
.WithOpenApi();

ticketsApi.MapPost("/", (CreateTicketRequest request, ITicketService ticketService) =>
{
    try
    {
        var createdTicket = ticketService.CreateTicket(request);

        return Results.Created($"/api/v2/tickets/{createdTicket.Id}", createdTicket);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new
        {
            error = ex.Message
        });
    }
})
.WithName("CreateV2Ticket")
.WithOpenApi();

ticketsApi.MapPut("/{id:int}", (int id, UpdateTicketRequest request, ITicketService ticketService) =>
{
    try
    {
        var updatedTicket = ticketService.UpdateTicket(id, request);

        return Results.Ok(updatedTicket);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new
        {
            error = ex.Message
        });
    }
})
.WithName("UpdateV2Ticket")
.WithOpenApi();

ticketsApi.MapDelete("/{id:int}", (int id, ITicketService ticketService) =>
{
    ticketService.DeleteTicket(id);

    return Results.NoContent();
})
.WithName("DeleteV2Ticket")
.WithOpenApi();

app.Run();

public partial class Program
{
}