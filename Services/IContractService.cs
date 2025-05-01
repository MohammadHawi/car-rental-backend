public interface IContractService 
{
    Task<(IEnumerable<ContractResponseDto> Items, int TotalCount)> GetAllContractsAsync(int pageNumber, int pageSize, string? searchQuery = null);
    Task<Contract> GetContractByIdAsync(int id);
    Task AddContractAsync(Contract contract);
    Task UpdateContractAsync(Contract contract);
    Task DeleteContractAsync(int id);
    Task<ContractResponseDto> CreateContractFromDtoAsync(ContractRequestDto dto);
    Task ReturnContract(int contractId, DateTime checkInDate);

    

}