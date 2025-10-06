namespace BAMF_API.DTOs.Requests.OrderDTOs
{
    public class OrderCreateDto
    {
        public string Email { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
