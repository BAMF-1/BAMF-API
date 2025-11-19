// BAMF_API/Models/InventoryTransactionType.cs
namespace BAMF_API.Models
{
	public enum InventoryTransactionType : byte
	{
		Restock = 0,
		Sale = 1,
		Adjustment = 2,
		Order = 3
	}
}
