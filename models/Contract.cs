using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Contract
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment ID
    public int Id { get; set; }

    // Foreign key reference to Car (using Car.Id, not Plate)
    public int CarId { get; set; }
    [ForeignKey("CarId")]
    public Car? Car { get; set; }

    // Foreign key reference to Customer
    public int Cid { get; set; }
    [ForeignKey("Cid")]
    public Customer? Customer { get; set; }

    public double Price { get; set; }
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public double? Deposit { get; set; }
    public int? Paid { get; set; }
    public string? Note { get; set; }
    public int? Returned { get; set; }   // 1 for returned, 0 for not returned
    public int Status { get; set; }   // 1 --> active  2--> overdue 3--> closed (then return = 1)
}
