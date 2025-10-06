using System.ComponentModel.DataAnnotations;

namespace BAMF_API.Models
{
    public class ProductCategory
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // Navigation
        public Product? Product { get; set; }
        public Category? Category { get; set; }
    }
}

// F.A