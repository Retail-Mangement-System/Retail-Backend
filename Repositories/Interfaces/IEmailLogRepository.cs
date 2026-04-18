using RetailOrdering.API.Models;

namespace RetailOrdering.API.Repositories.Interfaces
{
    public interface IEmailLogRepository
    {
        Task AddAsync(EmailLog log);
        Task<IEnumerable<EmailLog>> GetByUserIdAsync(int userId);
        Task<IEnumerable<EmailLog>> GetAllAsync();
    }
}
