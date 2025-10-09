
using System;
using System.ComponentModel.DataAnnotations;

namespace BAMF_API.DTOs.Requests;
public class CreateVariantRequest
{
    [Required, MaxLength(100)]
    public string Sku { get; set; } = null!;

    [Required]
    public Guid ProductGroupId { get; set; }

    [Required, MaxLength(60)]
    public string Color { get; set; } = null!;

    [Required, MaxLength(60)]
    public string Size { get; set; } = null!;

    [Range(0, 9999999)]
    public decimal Price { get; set; }
}
