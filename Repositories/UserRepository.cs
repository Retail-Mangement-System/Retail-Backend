using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.Models;
<<<<<<< HEAD
=======
using RetailOrdering.API.Repositories.Interfaces;
>>>>>>> origin/dev

namespace RetailOrdering.API.Repositories;

public class UserRepository : IUserRepository
{
<<<<<<< HEAD
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
=======
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email.ToLower().Trim());
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.UserId == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
>>>>>>> origin/dev
    }

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users
            .Include(u => u.LoyaltyAccount)
            .AsNoTracking()
            .ToListAsync();

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users
            .Include(u => u.LoyaltyAccount)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Users.AnyAsync(u => u.UserId == id);
}