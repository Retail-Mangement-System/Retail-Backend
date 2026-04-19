using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Common.Enums;
using RetailOrdering.API.Helpers;
using RetailOrdering.API.Models;

namespace RetailOrdering.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultAdminAsync(AppDbContext context)
        {
            // Skip if database has pending migrations (not created yet)
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                return;

            // Check if any admin already exists
            var adminExists = await context.Users
                .AnyAsync(u => u.Role == UserRole.Admin);

            if (adminExists)
                return;

            // Create default admin
            var admin = new User
            {
                FullName = "System Admin",
                Email = "admin@retail.com",
                PasswordHash = PasswordHelper.HashPassword("Admin@123"),
                PhoneNumber = "0000000000",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();

            // Create loyalty account for admin
            var loyalty = new LoyaltyAccount
            {
                UserId = admin.UserId,
                Points = 0,
            };
            context.LoyaltyAccounts.Add(loyalty);
            await context.SaveChangesAsync();
        }
    }
}

