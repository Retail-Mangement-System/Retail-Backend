using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Order;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService) => _orderService = orderService;

    // ── Helper ─────────────────────────────────────────────────────────────
    private int GetUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // POST api/orders/place
    [HttpPost("place")]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        var order = await _orderService.PlaceOrderAsync(GetUserId(), dto);
        return Ok(ApiResponse<OrderDto>.Ok(order, "Order placed successfully!"));
    }

    // GET api/orders/history
    [HttpGet("history")]
    public async Task<IActionResult> GetOrderHistory()
    {
        var orders = await _orderService.GetOrderHistoryAsync(GetUserId());
        return Ok(ApiResponse<List<OrderDto>>.Ok(orders));
    }

    // GET api/orders/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order is null)
            return NotFound(ApiResponse<OrderDto>.Fail("Order not found."));

        return Ok(ApiResponse<OrderDto>.Ok(order));
    }

    // Add to OrderController.cs
    // POST api/orders/{id}/reorder
    [HttpPost("{id:int}/reorder")]
    public async Task<IActionResult> Reorder(int id, [FromBody] ReorderRequest req)
    {
        var order = await _orderService.ReorderAsync(GetUserId(), id, req.ShippingAddress);
        return Ok(ApiResponse<OrderDto>.Ok(order, "Reorder placed successfully!"));
    }

    public class ReorderRequest
    {
        public string ShippingAddress { get; set; } = string.Empty;
    }
}