using System;
using System.Net.Sockets;
using UniDesc.Web.Models;
using UniDesc.Web.Services;
using Xunit;

namespace UniDesk.UnitTests.Models
{
    public class TicketTests
    {
        [Fact]
        public void Ticket_ShouldHaveStatusNew_WhenCreated()
        {
            var ticket = new Ticket();

            Assert.Equal(TicketStatus.New, ticket.Status);
            Assert.NotEqual(default(DateTime), ticket.CreatedAt);
        }
    }
}
