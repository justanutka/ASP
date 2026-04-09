using UniDesc.Web.Services;
using UniDesc.Web.Models;
using Xunit;
using System.Linq;

namespace UniDesk.UnitTests
{
    public class TicketServiceTests
    {
        [Fact]
        public void UpdateTicketStatus_ShouldChangeStatus()
        {
            var service = new InMemoryTicketService();
            var ticket = new Ticket { Title = "Test ticket" };

            service.AddTicket(ticket);

            service.UpdateTicketStatus(1, TicketStatus.Closed);
            var result = service.GetTicketById(1);

            Assert.Equal(TicketStatus.Closed, result.Status);
        }
    }
}