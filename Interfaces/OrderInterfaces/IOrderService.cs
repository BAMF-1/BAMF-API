using BAMF_API.DTOs.Requests.OrderDTOs;
using BAMF_API.Models;

namespace BAMF_API.Interfaces.OrderInterfaces
{
    public interface IOrderService
    {
        Task<Order?> GetOrderAsync(int id);
        Task<IEnumerable<Order>?> GetOrderByOrderEmailAsync(string email);
        Task<Order?> GetOrderByOrderNoAsync(string orderNr);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task CreateOrderAsync(OrderCreateDto dto);
        Task UpdateOrderAsync(int id, OrderUpdateDto dto);
        Task DeleteOrderAsync(int id);
    }
}
