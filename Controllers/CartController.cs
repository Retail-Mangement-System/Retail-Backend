using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Order;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) => _cartService = cartService;

    // ── Helper ─────────────────────────────────────────────────────────────
    private int GetUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET api/cart
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCartAsync(GetUserId());

        // Return empty cart shape instead of 404
        if (cart is null)
            cart = new CartDto { CartId = 0, Items = [], Total = 0 };

        return Ok(ApiResponse<CartDto>.Ok(cart));
    }

    // POST api/cart/add
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest req)
    {
        await _cartService.AddToCartAsync(GetUserId(), req.ProductId, req.Quantity);
        return Ok(ApiResponse<object>.Ok(null!, "Item added to cart."));
    }

    // DELETE api/cart/remove/{cartItemId}
    [HttpDelete("remove/{cartItemId:int}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        await _cartService.RemoveFromCartAsync(GetUserId(), cartItemId);
        return Ok(ApiResponse<object>.Ok(null!, "Item removed from cart."));
    }

    // DELETE api/cart/clear
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(GetUserId());
        return Ok(ApiResponse<object>.Ok(null!, "Cart cleared."));
    }
}

// ── Request DTO (inline — keep simple) ────────────────────────────────────
public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}