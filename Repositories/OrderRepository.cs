using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Data;
using BAMF_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BAMF_API.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _ctx;
    public OrderRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _ctx.Orders.AddAsync(order, ct);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct = default)
    {
        var o = await _ctx.Orders.FindAsync(new object[]{id}, ct);
        if (o != null) _ctx.Orders.Remove(o);
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _ctx.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken ct = default)
    {
        return await _ctx.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.OrderNo == orderNo, ct);
    }

    public async Task<List<Order>> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _ctx.Orders.Include(o => o.Lines).Where(o => o.Email == email).ToListAsync(ct);
    }

    public async Task<List<Order>> ListAllAsync(CancellationToken ct = default)
    {
        return await _ctx.Orders.Include(o => o.Lines).OrderByDescending(o => o.CreatedAt).ToListAsync(ct);
    }

    public void Update(Order order)
    {
        _ctx.Orders.Update(order);
    }
}