using System;
using System.ComponentModel.DataAnnotations;

namespace BAMF_API.Models 
{ 
    public class Inventory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VariantId { get; set; }
        public Variant Variant { get; set; } = null!;

        public int Quantity { get; set; } = 0;
        public int LowStockThreshold { get; set; } = 0;
        public DateTimeOffset? LastRestockDate { get; set; }

        public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    }
}
