namespace UniDesc.Web.Models
{
    public class TicketQueryParameters
    {
        public string? Status { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortDirection { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}