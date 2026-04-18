using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Promotion;
using RetailOrdering.API.Services.Interfaces;
using System.Security.Claims;

namespace RetailOrdering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        // ── POST api/promotions/validate-coupon ───────────────────────────────────
        [HttpPost("validate-coupon")]
        public async Task<IActionResult> ValidateCoupon([FromBody] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(ApiResponse<object>.Fail("Coupon code is required."));

            var coupon = await _promotionService.ValidateCouponAsync(code);

            if (coupon is null)
                return NotFound(ApiResponse<object>.Fail("Coupon is invalid, expired, or inactive."));

            return Ok(ApiResponse<CouponDto>.Ok(coupon, "Coupon is valid."));
        }

        // ── GET api/promotions/loyalty ────────────────────────────────────────────
        [HttpGet("loyalty")]
        public async Task<IActionResult> GetLoyaltyPoints()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var loyalty = await _promotionService.GetLoyaltyPointsAsync(userId);

            if (loyalty is null)
                return NotFound(ApiResponse<object>.Fail("No loyalty account found for this user."));

            return Ok(ApiResponse<LoyaltyDto>.Ok(loyalty, "Loyalty points retrieved."));
        }

        // ── POST api/promotions/loyalty/add ──────────────────────────────────────
        // Admin only — typically called internally by OrderService after confirmation
        [HttpPost("loyalty/add")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddLoyaltyPoints(
            [FromQuery] int userId,
            [FromQuery] int points)
        {
            if (points <= 0)
                return BadRequest(ApiResponse<object>.Fail("Points must be a positive number."));

            await _promotionService.AddLoyaltyPointsAsync(userId, points);

            return Ok(ApiResponse<object>.Ok(null!, $"{points} loyalty points added to user {userId}."));
        }
    }
}
