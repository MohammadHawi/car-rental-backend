public class CarHistoryDto
{
    public int CarId { get; set; }
    public string? CarDetails { get; set; }
    public IEnumerable<ContractResponseDto> Contracts { get; set; } = new List<ContractResponseDto>();
    public IEnumerable<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    public FinancialSummaryDto Summary { get; set; } = new FinancialSummaryDto();
}