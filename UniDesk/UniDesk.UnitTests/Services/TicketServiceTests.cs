using UniDesc.Web.Services;
using UniDesc.Web.Models;
using Xunit;
using System.Linq;

namespace UniDesk.UnitTests
{
    public class TicketServiceTests
    {
        [Fact]
        public void UpdateTicketStatus_ShouldChangeStatus_WhenStatusIsValid()
        {
            var service = new InMemoryTicketService();
            var ticket = new Ticket { Title = "Test ticket" };

            service.AddTicket(ticket);

            service.UpdateTicketStatus(1, TicketStatus.Closed);
            var result = service.GetTicketById(1);

            Assert.Equal(TicketStatus.Closed, result.Status);
        }

        [Fact]
        public void UpdateTicketStatus_ShouldThrowException_WhenTicketIsAlreadyClosed()
        {
            var service = new InMemoryTicketService();
            var ticket = new Ticket
            {
                Title = "Closed ticket",
                Status = TicketStatus.Closed
            };

            service.AddTicket(ticket);

            var exception = Assert.Throws<InvalidOperationException>(
                () => service.UpdateTicketStatus(1, TicketStatus.Closed)
            );

            Assert.Equal("Ticket is already closed.", exception.Message);
        }

        [Fact]
        public void GetTickets_ShouldReturnSecondPage_WhenPageIs2AndPageSizeIs10()
        {
            var service = new InMemoryTicketService();

            for (int i = 1; i <= 15; i++)
            {
                service.AddTicket(new Ticket
                {
                    Title = $"Ticket {i}"
                });
            }

            var queryParams = new TicketQueryParameters
            {
                Page = 2,
                PageSize = 10
            };

            var result = service.GetTickets(queryParams).ToList();

            Assert.Equal(5, result.Count);
            Assert.Equal("Ticket 11", result[0].Title);
        }

        [Fact]
        public void GetTickets_ShouldReturnFirstPage_WhenPageIs1AndPageSizeIs10()
        {
            var service = new InMemoryTicketService();

            for (int i = 1; i <= 15; i++)
            {
                service.AddTicket(new Ticket
                {
                    Title = $"Ticket {i}"
                });
            }

            var queryParams = new TicketQueryParameters
            {
                Page = 1,
                PageSize = 10
            };

            var result = service.GetTickets(queryParams).ToList();

            Assert.Equal(10, result.Count);
            Assert.Equal("Ticket 1", result[0].Title);
        }
    }
}