using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Car
{
    [Key]  // Marks Id as the Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int Id { get; set; }

    [Required]
    public int Plate { get; set; }

    public string? Brand { get; set; }
    
    public string? Class { get; set; }

    public int? Model { get; set; }

    public string? Color { get; set; }
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();  // A car can have many contracts

}
