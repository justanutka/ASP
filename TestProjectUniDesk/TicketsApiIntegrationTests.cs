using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TestProjectUniDesk
{
    public class TicketsApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _client;

        public TicketsApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTickets_ShouldReturnOkAndTicketList()
        {
            var response = await _client.GetAsync("/api/tickets");

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseBody));

            var tickets = JsonSerializer.Deserialize<List<TicketResponse>>(responseBody, JsonOptions);
            Assert.NotNull(tickets);
            Assert.Contains(tickets, ticket => ticket.Title == "First seeded ticket");
            Assert.Contains(tickets, ticket => ticket.Title == "Second seeded ticket");
        }

        [Fact]
        public async Task CreateTicket_ShouldReturnCreated_WhenInputIsValid()
        {
            var newTicket = new
            {
                Title = "New Ticket",
                Description = "Ticket description",
                Status = "New"
            };

            var response = await _client.PostAsJsonAsync("/api/tickets", newTicket);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdTicket = await response.Content.ReadFromJsonAsync<TicketResponse>();
            Assert.NotNull(createdTicket);
            Assert.Equal("New Ticket", createdTicket!.Title);
            Assert.Equal("New", createdTicket.Status);
        }

        [Fact]
        public async Task CreateTicket_ShouldReturnBadRequest_WhenTitleIsMissing()
        {
            var newTicket = new
            {
                Description = "Ticket description without title",
                Status = "New"
            };

            var response = await _client.PostAsJsonAsync("/api/tickets", newTicket);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetTicketById_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            var response = await _client.GetAsync("/api/tickets/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTicketStatus_ShouldReturnOk_WhenStatusUpdated()
        {
            var response = await _client.PatchAsync(
                "/api/tickets/1/status",
                JsonContent.Create(new { Status = "Closed" }));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedTicket = await response.Content.ReadFromJsonAsync<TicketResponse>();
            Assert.NotNull(updatedTicket);
            Assert.Equal(1, updatedTicket!.Id);
            Assert.Equal("Closed", updatedTicket.Status);

            var persistedResponse = await _client.GetAsync("/api/tickets/1");
            persistedResponse.EnsureSuccessStatusCode();

            var persistedTicket = await persistedResponse.Content.ReadFromJsonAsync<TicketResponse>();
            Assert.NotNull(persistedTicket);
            Assert.Equal("Closed", persistedTicket!.Status);
        }

        private sealed record TicketResponse(int Id, string Title, string Status);
    }
}
