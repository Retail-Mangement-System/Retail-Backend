using RetailOrdering.API.DTOs.Promotion;

namespace RetailOrdering.API.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<CouponDto?> ValidateCouponAsync(string code);
        Task<LoyaltyDto?> GetLoyaltyPointsAsync(int userId);
        Task AddLoyaltyPointsAsync(int userId, int points);
    }
}
