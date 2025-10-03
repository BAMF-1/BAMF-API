using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    [Required]
    public required string PasswordSalt { get; set; }

    public ICollection<CartItem> Cart { get; init; } = new List<CartItem>();
}

public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    // Foreign Key
    public int UserId { get; set; }

    public required User User { get; set; }
}
