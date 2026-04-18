using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.Data;
using RetailOrdering.API.DTOs.Promotion;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class NotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IEmailLogRepository _emailLogRepo;
        private readonly AppDbContext _context;

        public NotificationController(
            IEmailService emailService,
            IEmailLogRepository emailLogRepo,
            AppDbContext context)
        {
            _emailService = emailService;
            _emailLogRepo = emailLogRepo;
            _context = context;
        }

        // ── POST api/notifications/resend-confirmation/{orderId} ─────────────────
        [HttpPost("resend-confirmation/{orderId:int}")]
        public async Task<IActionResult> ResendOrderConfirmation(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order is null)
                return NotFound(ApiResponse<object>.Fail($"Order {orderId} not found."));

            await _emailService.SendOrderConfirmationAsync(order);

            return Ok(ApiResponse<object>.Ok(null!, $"Confirmation email sent for Order #{orderId}."));
        }

        // ── GET api/notifications/logs ────────────────────────────────────────────
        [HttpGet("logs")]
        public async Task<IActionResult> GetAllEmailLogs()
        {
            var logs = await _emailLogRepo.GetAllAsync();

            var result = logs.Select(l => new EmailLogDto
            {
                EmailLogId = l.EmailLogId,
                UserId = l.UserId,
                OrderId = l.OrderId,
                EmailType = l.EmailType,
                SentAt = l.SentAt,
                Status = l.Status,
                ErrorMessage = l.ErrorMessage
            });

            return Ok(ApiResponse<IEnumerable<EmailLogDto>>.Ok(result, "Email logs retrieved."));
        }
    }
}
