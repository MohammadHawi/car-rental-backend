public class ContractRequestDto
{
    public string? Car { get; set; } // Plate number
    public string? Name { get; set; } // Existing customer's full name

    public string? FirstName { get; set; }  // For new customer
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public int? NationalityId { get; set; }
    public string? PhoneNumber { get; set; }

    public DateTime PickOut { get; set; }
    public DateTime DropIn { get; set; }
    public double Price { get; set; }
    public double? Deposit { get; set; }
    public int? Paid { get; set; }
    public int returned { get; set; }     
}
