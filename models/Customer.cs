using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment ID
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)] 
    public string? MiddleName { get; set; } // Optional

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    // Foreign key reference to Nationality table
    [Required]
    public int NationalityId { get; set; }

    [ForeignKey("NationalityId")]
    public Nationality? Nationality { get; set; }

    // Passport Image (Stored as Base64 or File Path)
    public byte[]? PassportImage { get; set; }

    // Driver License Image (Stored as Base64 or File Path)
    public byte[]? LicenseImage { get; set; }
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();  // A customer can have many contracts

}
