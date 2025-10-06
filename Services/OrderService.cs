using BAMF_API.DTOs.Requests.OrderDTOs;
using BAMF_API.Interfaces.OrderInterfaces;
using BAMF_API.Models;

namespace BAMF_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepo.GetAllAsync();
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await _orderRepo.GetByIdAsync(id);
        }

        public async Task CreateOrderAsync(OrderCreateDto dto)
        {
            // TODO: Check for products existing before adding order, or it will throw error
            var orderNo = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";

            var order = new Order
            {
                OrderNo = orderNo[..Math.Min(45, orderNo.Length)],
                Email = dto.Email,
                Total = dto.Total,
                Status = "Pending",
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    VariantId = i.VariantId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _orderRepo.CreateAsync(order);
        }

        public async Task UpdateOrderAsync(int id, OrderUpdateDto dto)
        {
            // TODO: Check for products existing before updating order, or it will throw error
            var existingOrder = await _orderRepo.GetByIdAsync(id);
            if (existingOrder == null)
                throw new Exception("Order not found");

            if (!string.IsNullOrWhiteSpace(dto.Status))
                existingOrder.Status = dto.Status;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                existingOrder.Email = dto.Email;

            if (dto.Total.HasValue)
                existingOrder.Total = dto.Total.Value;

            await _orderRepo.UpdateAsync(existingOrder);
        }

        public Task DeleteOrderAsync(int id)
        {
            var existingOrder = _orderRepo.GetByIdAsync(id);
            if (existingOrder == null)
                throw new Exception("Order not found");
            return _orderRepo.DeleteAsync(id);
        }
    }
}