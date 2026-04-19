using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.DTOs.Promotion;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly ICouponRepository _couponRepo;
        private readonly AppDbContext _context;

        public PromotionService(ICouponRepository couponRepo, AppDbContext context)
        {
            _couponRepo = couponRepo;
            _context = context;
        }

        // ── Validate Coupon ───────────────────────────────────────────────────────

        public async Task<CouponDto?> ValidateCouponAsync(string code)
        {
            var coupon = await _couponRepo.GetByCodeAsync(code);

            if (coupon is null || !coupon.IsActive) return null;
            if (coupon.ExpiryDate < DateTime.UtcNow) return null;

            return new CouponDto
            {
                Code = coupon.Code,
                DiscountPercent = coupon.DiscountPercent,
                ExpiryDate = coupon.ExpiryDate
            };
        }

        // ── Loyalty Points ────────────────────────────────────────────────────────

        public async Task<LoyaltyDto?> GetLoyaltyPointsAsync(int userId)
        {
            var account = await _context.LoyaltyAccounts
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (account is null) return null;

            return new LoyaltyDto
            {
                Points = account.Points,
                LastUpdated = account.LastUpdated
            };
        }

        public async Task AddLoyaltyPointsAsync(int userId, int points)
        {
            var account = await _context.LoyaltyAccounts
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (account is null)
            {
                _context.LoyaltyAccounts.Add(new LoyaltyAccount
                {
                    UserId = userId,
                    Points = points,
                    LastUpdated = DateTime.UtcNow
                });
            }
            else
            {
                account.Points += points;
                account.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
