
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BAMF_API.Models;

public class Variant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // SKU globally unique
    [Required, MaxLength(100)]
    public string Sku { get; set; } = null!;

    [Required, MaxLength(60)]
    public string Color { get; set; } = null!;

    [Required, MaxLength(60)]
    public string Size { get; set; } = null!;

    public decimal Price { get; set; }

    public Guid ProductGroupId { get; set; }
    public ProductGroup ProductGroup { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;

    public Inventory Inventory { get; set; } = null!;
    public ICollection<VariantImage> VariantImages { get; set; } = new List<VariantImage>();
}
