public class ContractResponseDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int Cid { get; set; }
    public string? CarPlate { get; set; }
    public string? CustomerName { get; set; }
    public double Price { get; set; }
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public double? Deposit { get; set; }
    public int? Paid { get; set; }
    public int Status { get; set; }
    public string? PhoneNumber { get; set; }
    
    public int Total { get; set; }
    public int Balance { get; set; }
}
