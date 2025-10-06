using BAMF_API.Models;

namespace BAMF_API.Interfaces.OrderInterfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }

}
