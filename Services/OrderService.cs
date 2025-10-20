using BAMF_API.DTOs.Requests.OrderDTOs;
using BAMF_API.Interfaces.OrderInterfaces;
using BAMF_API.Interfaces.InventoryInterfaces;
using BAMF_API.Interfaces.ProductInterfaces;
using BAMF_API.Models;

namespace BAMF_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IInventoryService _inventoryService;
        private readonly IVariantRepository _variantRepository;

        public OrderService(
            IOrderRepository orderRepo,
            IInventoryService inventoryService,
            IVariantRepository variantRepository)
        {
            _orderRepo = orderRepo;
            _inventoryService = inventoryService;
            _variantRepository = variantRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepo.GetAllAsync();
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await _orderRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Order>?> GetOrderByOrderEmailAsync(string email)
        {
            return await _orderRepo.GetOrderByEmailAsync(email);
        }

        public async Task<Order?> GetOrderByOrderNoAsync(string orderNo)
        {
            return await _orderRepo.GetOrderByOrderNoAsync(orderNo);
        }

        public async Task CreateOrderAsync(OrderCreateDto dto)
        {
            // Generate a unique order number and trim to 45 characters.
            var orderNo = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
            orderNo = orderNo[..Math.Min(45, orderNo.Length)];

            decimal total = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                // Look up the variant by SKU.
                var variant = await _variantRepository.GetBySkuAsync(item.Sku);
                if (variant == null)
                {
                    throw new InvalidOperationException($"Variant with SKU {item.Sku} not found.");
                }

                // Compute price.
                var unitPrice = variant.Price;
                total += unitPrice * item.Quantity;

                // Decrement inventory and record a sale transaction.
                await _inventoryService.AdjustInventoryAsync(
                    variant.Sku,
                    -item.Quantity,
                    InventoryTransactionType.Sale,
                    referenceId: orderNo);

                // Build the order item entity with the resolved price.
                orderItems.Add(new OrderItem
                {
                    VariantId = variant.Id,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
            }

            // Assemble the order and save it.
            var order = new Order
            {
                OrderNo = orderNo,
                Email = dto.Email,
                Status = "Pending",
                Total = total,
                CreatedUtc = DateTime.UtcNow,
                Items = orderItems
            };

            await _orderRepo.CreateAsync(order);
        }

        public async Task UpdateOrderAsync(int id, OrderUpdateDto dto)
        {
            var existingOrder = await _orderRepo.GetByIdAsync(id);
            if (existingOrder == null)
                throw new KeyNotFoundException("Order not found.");

            if (!string.IsNullOrWhiteSpace(dto.Status))
                existingOrder.Status = dto.Status;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                existingOrder.Email = dto.Email;

            if (dto.Total.HasValue)
                existingOrder.Total = dto.Total.Value;

            await _orderRepo.UpdateAsync(existingOrder);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var existingOrder = await _orderRepo.GetByIdAsync(id);
            if (existingOrder == null)
                throw new KeyNotFoundException("Order not found.");

            await _orderRepo.DeleteAsync(id);
        }
    }
}
