namespace RetailOrdering.API.Models;

public class LoyaltyAccount
{
    public int LoyaltyAccountId { get; set; }   // ✅ EF will detect this automatically
    public int UserId { get; set; }
    public int Points { get; set; } = 0;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}
