using System.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using UniDesc.Web;
using UniDesc.Web.Models;

namespace TestProjectUniDesk
{
    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection = new("Data Source=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureLogging(logging => logging.ClearProviders());

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<UniDeskDbContext>();
                services.RemoveAll<DbContextOptions<UniDeskDbContext>>();

                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                services.AddDbContext<UniDeskDbContext>(options =>
                    options.UseSqlite(_connection));

                using var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<UniDeskDbContext>();

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                SeedTickets(dbContext);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection.Dispose();
            }

            base.Dispose(disposing);
        }

        private static void SeedTickets(UniDeskDbContext dbContext)
        {
            dbContext.Tickets.AddRange(
                new Ticket
                {
                    Id = 1,
                    Title = "First seeded ticket",
                    Description = "Seeded integration test ticket",
                    Status = TicketStatus.New,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = 2,
                    Title = "Second seeded ticket",
                    Description = "Another seeded integration test ticket",
                    Status = TicketStatus.InProgress,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

            dbContext.SaveChanges();
        }
    }
}
