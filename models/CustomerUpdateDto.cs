using System.ComponentModel.DataAnnotations;

public class CustomerUpdateDto
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? MiddleName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public int NationalityId { get; set; }

    public byte[]? PassportImage { get; set; }
    public byte[]? LicenseImage { get; set; }
}
