using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAMF_API.Models
{
    public class Variant
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [Required, MaxLength(50)]
        public required string Sku { get; set; } // Stock Keeping Unit identifier

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalPrice { get; set; } // Price adjustment from base product price

        [Column(TypeName = "nvarchar(max)")]
        public string? Attributes { get; set; } // e.g. {"Color": "Red", "Size": "M"}

        // Navigation
        public Product? Product { get; set; }
        public Inventory? Inventory { get; set; }
    }
}

