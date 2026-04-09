namespace UniDesc.Web.DTOs
{
    public class TicketListDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Status { get; set; }
    }
}