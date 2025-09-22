using System.ComponentModel.DataAnnotations;

public class TransactionCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    // Optional fields for more flexibility
    public string? Description { get; set; }

    // e.g., Income or Expense
    public string? Type { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
