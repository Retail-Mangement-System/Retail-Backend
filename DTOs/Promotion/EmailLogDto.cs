namespace RetailOrdering.API.DTOs.Promotion;

public class EmailLogDto
{
    public int EmailLogId { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string EmailType { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}