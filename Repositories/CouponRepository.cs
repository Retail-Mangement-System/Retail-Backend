using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;

namespace RetailOrdering.API.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _context;

        public CouponRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
            => await _context.Coupons
                   .FirstOrDefaultAsync(c => c.Code == code.ToUpper().Trim());

        public async Task<Coupon?> GetByIdAsync(int id)
            => await _context.Coupons.FindAsync(id);

        public async Task<IEnumerable<Coupon>> GetAllAsync()
            => await _context.Coupons.ToListAsync();

        public async Task<Coupon> CreateAsync(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
        }
    }
}
