using RetailOrdering.API.Common.Enums;
using RetailOrdering.API.DTOs.Order;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICartRepository _cartRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly ICouponRepository _couponRepo;
    private readonly IEmailService _emailService;

    public OrderService(
        IOrderRepository orderRepo,
        ICartRepository cartRepo,
        IInventoryRepository inventoryRepo,
        ICouponRepository couponRepo,
        IEmailService emailService)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _inventoryRepo = inventoryRepo;
        _couponRepo = couponRepo;
        _emailService = emailService;
    }

    // PLACE ORDER ──────────────────────────────────────────────────────────
    public async Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderDto dto)
    {
        // 1. Load cart
        var cart = await _cartRepo.GetCartByUserIdAsync(userId);
        if (cart is null || !cart.CartItems.Any())
            throw new ArgumentException("Cart is empty. Add items before placing an order.");

        // 2. Build order items + validate stock
        var orderItems = new List<OrderItem>();
        foreach (var ci in cart.CartItems)
        {
            var inv = await _inventoryRepo.GetByProductIdAsync(ci.ProductId)
                ?? throw new KeyNotFoundException($"Inventory not found for product {ci.ProductId}.");

            if (inv.QuantityInStock < ci.Quantity)
                throw new ArgumentException(
                    $"Insufficient stock for '{ci.Product?.Name}'. Available: {inv.QuantityInStock}.");

            orderItems.Add(new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product!.Price
            });
        }

        // 3. Calculate total
        decimal total = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

        // 4. Apply coupon discount (optional)
        if (!string.IsNullOrWhiteSpace(dto.CouponCode))
        {
            var coupon = await _couponRepo.GetByCodeAsync(dto.CouponCode);
            if (coupon is not null && coupon.IsActive && coupon.ExpiryDate >= DateTime.UtcNow)
            {
                var discount = total * (coupon.DiscountPercent / 100);
                total -= discount;
            }
            // Silently ignore invalid coupons (don't block the order)
        }

        // 5. Create order
        var order = new Order
        {
            UserId = userId,
            TotalAmount = total,
            Status = OrderStatus.Confirmed,
            ShippingAddress = dto.ShippingAddress,
            PlacedAt = DateTime.UtcNow,
            OrderItems = orderItems
        };

        await _orderRepo.CreateOrderAsync(order);

        // 6. Deduct inventory
        foreach (var oi in orderItems)
        {
            await _inventoryRepo.UpdateStockAsync(oi.ProductId, -oi.Quantity);
        }

        // 7. Clear cart
        await _cartRepo.ClearCartAsync(cart.CartId);
        var user = cart.User;
        if (user is not null)
            await _emailService.SendOrderConfirmationAsync(order);

        return MapOrderToDto(order);
    }

    // ORDER HISTORY ────────────────────────────────────────────────────────
    public async Task<List<OrderDto>> GetOrderHistoryAsync(int userId)
    {
        var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
        return orders.Select(MapOrderToDto).ToList();
    }

    // GET ORDER BY ID ──────────────────────────────────────────────────────
    public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
    {
        var order = await _orderRepo.GetOrderByIdAsync(orderId);
        return order is null ? null : MapOrderToDto(order);
    }

    // HELPER ───────────────────────────────────────────────────────────────
    private static OrderDto MapOrderToDto(Order order)
    {
        var items = order.OrderItems.Select(oi => new CartItemDto
        {
            ProductId = oi.ProductId,
            ProductName = oi.Product?.Name ?? string.Empty,
            Price = oi.UnitPrice,
            Quantity = oi.Quantity,
            Subtotal = oi.UnitPrice * oi.Quantity
        }).ToList();

        return new OrderDto
        {
            OrderId = order.OrderId,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            ShippingAddress = order.ShippingAddress,
            PlacedAt = order.PlacedAt,
            Items = items
        };
    }

    // Add to OrderService.cs
    public async Task<OrderDto> ReorderAsync(int userId, int orderId, string shippingAddress)
    {
        var original = await _orderRepo.GetOrderByIdAsync(orderId)
            ?? throw new KeyNotFoundException("Order not found.");

        if (original.UserId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        // Clear current cart and re-add the same items
        await _cartRepo.ClearCartAsync((await _cartRepo.GetOrCreateCartAsync(userId)).CartId);

        foreach (var oi in original.OrderItems)
        {
            await _cartRepo.GetOrCreateCartAsync(userId).ContinueWith(async t =>
            {
                var cart = t.Result;
                var existing = await _cartRepo.GetCartItemAsync(cart.CartId, oi.ProductId);
                if (existing is null)
                    await _cartRepo.AddCartItemAsync(new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity
                    });
            });
        }

        // Place a new order from the repopulated cart
        return await PlaceOrderAsync(userId, new PlaceOrderDto
        {
            ShippingAddress = shippingAddress
        });
    }
}