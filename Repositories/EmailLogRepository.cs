using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;

namespace RetailOrdering.API.Repositories
{
    public class EmailLogRepository : IEmailLogRepository
    {
        private readonly AppDbContext _context;

        public EmailLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailLog log)
        {
            _context.EmailLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmailLog>> GetByUserIdAsync(int userId)
            => await _context.EmailLogs
                   .Where(e => e.UserId == userId)
                   .OrderByDescending(e => e.SentAt)
                   .ToListAsync();

        public async Task<IEnumerable<EmailLog>> GetAllAsync()
            => await _context.EmailLogs
                   .OrderByDescending(e => e.SentAt)
                   .ToListAsync();
    }
}
