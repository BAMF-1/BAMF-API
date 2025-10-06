using BAMF_API.DTOs.Requests.OrderDTOs;
using BAMF_API.Interfaces.OrderInterfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        if (orders == null) return NotFound("There are no orders registered");
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
    {
        await _orderService.CreateOrderAsync(dto);
        return Ok("Order created");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderUpdateDto dto)
    {
        await _orderService.UpdateOrderAsync(id, dto);
        return Ok("Updated");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        return Ok("Deleted");
    }
}
