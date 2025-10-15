using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BAMF_API.Models;

public enum OrderStatus { Created = 0, Paid = 1, Cancelled = 2, Shipped = 3 }

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string OrderNo { get; set; } = null!;

    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }

    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }

    public List<OrderLine> Lines { get; set; } = new();
}

public class OrderLine
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid VariantId { get; set; }
    public string Sku { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}