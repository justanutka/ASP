using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace TestProjectUniDesk
{
    public class TicketSecurityTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TicketSecurityTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTicket_ShouldReturnBadRequest_WhenTitleIsEmpty()
        {
            var invalidTicket = new
            {
                title = "",
                status = "New"
            };

            var response = await _client.PostAsJsonAsync("/api/tickets", invalidTicket);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateTicket_ShouldReturnBadRequest_WhenTitleIsTooShort()
        {
            var invalidTicket = new
            {
                title = "A",
                status = "New"
            };

            var response = await _client.PostAsJsonAsync("/api/tickets", invalidTicket);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateTicket_ShouldReturnBadRequest_WhenStatusIsInvalid()
        {
            var invalidTicket = new
            {
                title = "Invalid status test",
                status = "WrongStatus"
            };

            var response = await _client.PostAsJsonAsync("/api/tickets", invalidTicket);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}