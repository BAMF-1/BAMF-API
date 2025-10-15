using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BAMF_API.DTOs.Requests.Orders;

public class OrderLineCreateDto
{
    [Required]
    public string Sku { get; set; } = null!;
    [Required]
    public int Quantity { get; set; }
}

public class OrderCreateDto
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public List<OrderLineCreateDto> Lines { get; set; } = new();
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
}