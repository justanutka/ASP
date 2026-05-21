using UniDesk.Web.DTOs;
using UniDesk.Web.Filters;
using UniDesk.Web.Services;
using System.Security.Claims;

namespace UniDesk.Web.Endpoints
{
    public static class TicketEndpoints
    {
        public static IEndpointRouteBuilder MapTicketEndpoints(this IEndpointRouteBuilder app)
        {
            var ticketsApi = app.MapGroup("/api/v2/tickets")
                .WithTags("Tickets v2")
                .RequireAuthorization()
                .AddEndpointFilter<RequestTimingFilter>();

            ticketsApi.MapGet("/current-user", (ClaimsPrincipal user) =>
            {
                var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.Identity?.Name;
                var employeeId = user.FindFirst(IdentitySeedData.EmployeeIdClaimType)?.Value;

                return Results.Ok(new
                {
                    email,
                    employeeId,
                    isAuthenticated = user.Identity?.IsAuthenticated == true,
                    isAdmin = user.IsInRole(IdentitySeedData.AdminRole)
                });
            })
            .RequireAuthorization("TopUniEmailOnly")
            .WithName("GetCurrentUserV2")
            .WithOpenApi();

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
            .AddEndpointFilter<ValidationFilter<CreateTicketRequest>>()
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
            .AddEndpointFilter<ValidationFilter<UpdateTicketRequest>>()
            .WithName("UpdateV2Ticket")
            .WithOpenApi();

            ticketsApi.MapDelete("/{id:int}", (int id, ITicketService ticketService) =>
            {
                ticketService.DeleteTicket(id);

                return Results.NoContent();
            })
            .RequireAuthorization(policy =>
            {
                policy.RequireRole(IdentitySeedData.AdminRole);
            })
            .WithName("DeleteV2Ticket")
            .WithOpenApi();

            return app;
        }
    }
}