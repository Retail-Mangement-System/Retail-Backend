using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.Helpers;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace RetailOrdering.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailLogRepository _emailLogRepo;
        private readonly AppDbContext _context;

        public EmailService(
            IConfiguration config,
            IEmailLogRepository emailLogRepo,
            AppDbContext context)
        {
            _config = config;
            _emailLogRepo = emailLogRepo;
            _context = context;
        }

        public async Task SendOrderConfirmationAsync(Order order)
        {
            // Always load full navigation properties
            var fullOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            if (fullOrder is null) return;

            var es = _config.GetSection("EmailSettings");
            var log = new EmailLog
            {
                UserId = fullOrder.UserId,
                OrderId = fullOrder.OrderId,
                EmailType = "OrderConfirmation",
                SentAt = DateTime.UtcNow,
                Status = "Sent"
            };

            try
            {
                var html = EmailTemplateHelper.OrderConfirmation(fullOrder, fullOrder.User);

                using var client = new SmtpClient(es["SmtpHost"])
                {
                    Port = int.Parse(es["SmtpPort"]!),
                    Credentials = new NetworkCredential(es["SenderEmail"], es["Password"]),
                    EnableSsl = true
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(es["SenderEmail"]!, es["SenderName"]),
                    Subject = $"Order #{fullOrder.OrderId} Confirmed — Retail Ordering",
                    Body = html,
                    IsBodyHtml = true
                };
                mail.To.Add(fullOrder.User.Email);

                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                log.Status = "Failed";
                log.ErrorMessage = ex.Message;
            }
            finally
            {
                await _emailLogRepo.AddAsync(log);
            }
        }
    }
}
