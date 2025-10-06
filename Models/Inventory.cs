using System.ComponentModel.DataAnnotations;

namespace BAMF_API.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        [Required]
        public int VariantId { get; set; }

        public bool InStock { get; set; } = true;

        [Range(0, 100000)]
        public int Amount { get; set; }

        // Navigation
        public Variant? Variant { get; set; }
    }
}

