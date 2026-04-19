using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Models;

namespace RetailOrdering.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── Module 1 — Auth ──────────────────────────────────────────────────────
    public DbSet<User> Users { get; set; }

    // ── Module 2 — Products ──────────────────────────────────────────────────
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Inventory> Inventories { get; set; }

    // ── Module 3 — Cart & Orders ─────────────────────────────────────────────
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    // ── Module 4 — Promotions ────────────────────────────────────────────────
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User — unique email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Product → Category (restrict delete)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product → Brand (restrict delete)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // Inventory → Product (1:1)
        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Product)
            .WithOne(p => p.Inventory)
            .HasForeignKey<Inventory>(i => i.ProductId);

        // Cart → User
        modelBuilder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId);

        // CartItem → Cart
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId);

        // CartItem → Product
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId);

        // Order → User
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId);

        // OrderItem → Order
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        // OrderItem → Product
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);

        // LoyaltyAccount → User (1:1)
        modelBuilder.Entity<LoyaltyAccount>()
            .HasOne(la => la.User)
            .WithOne(u => u.LoyaltyAccount)
            .HasForeignKey<LoyaltyAccount>(la => la.UserId);

        // Coupon — unique code
        modelBuilder.Entity<Coupon>()
            .HasIndex(c => c.Code)
            .IsUnique();

        // EmailLog → User & Order
        modelBuilder.Entity<EmailLog>()
            .HasOne(el => el.User)
            .WithMany(u => u.EmailLogs)
            .HasForeignKey(el => el.UserId);

        modelBuilder.Entity<EmailLog>()
            .HasOne(el => el.Order)
            .WithMany(o => o.EmailLogs)
            .HasForeignKey(el => el.OrderId);

        // Decimal precision
        modelBuilder.Entity<Product>()
            .Property(p => p.Price).HasPrecision(10, 2);
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount).HasPrecision(10, 2);
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice).HasPrecision(10, 2);
        modelBuilder.Entity<Coupon>()
            .Property(c => c.DiscountPercent).HasPrecision(5, 2);


        modelBuilder.Entity<LoyaltyAccount>()
            .HasKey(la => la.LoyaltyAccountId);              // ← add this line

        // The existing 1:1 relationship config stays as-is below it:
        modelBuilder.Entity<LoyaltyAccount>()
            .HasOne(la => la.User)
            .WithOne(u => u.LoyaltyAccount)
            .HasForeignKey<LoyaltyAccount>(la => la.UserId);
    }
}
