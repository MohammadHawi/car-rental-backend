using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CarStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CarId { get; set; }

    [ForeignKey("CarId")]
    public Car? Car { get; set; }

    [Required]
    public int Status { get; set; } // 0 = available, 1 = rented, 2 = in maintenance

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}