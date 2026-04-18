using RetailOrdering.API.Models;

namespace RetailOrdering.API.Repositories.Interfaces
{
    public interface ICouponRepository
    {
        Task<Coupon?> GetByCodeAsync(string code);
        Task<Coupon?> GetByIdAsync(int id);
        Task<IEnumerable<Coupon>> GetAllAsync();
        Task<Coupon> CreateAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
    }
}
