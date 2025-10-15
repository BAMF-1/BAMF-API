using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Models;
using BAMF_API.DTOs.Requests.Orders;

namespace BAMF_API.Services;

public interface IOrdersService
{
    Task<int> CreateOrderAsync(OrderCreateDto dto, CancellationToken ct = default);
    Task UpdateOrderAsync(int id, object dto, CancellationToken ct = default);
    Task DeleteOrderAsync(int id, CancellationToken ct = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken ct = default);
    Task<List<Order>> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<List<Order>> ListAllAsync(CancellationToken ct = default);
}