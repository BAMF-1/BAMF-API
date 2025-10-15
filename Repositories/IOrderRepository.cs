using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Models;

namespace BAMF_API.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken ct = default);
    Task<List<Order>> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<List<Order>> ListAllAsync(CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    void Update(Order order);
    Task DeleteByIdAsync(int id, CancellationToken ct = default);
}