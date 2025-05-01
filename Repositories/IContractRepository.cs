public interface IContractRepository
{
    Task<(IEnumerable<ContractResponseDto> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchQuery = null);
    Task<Contract> GetByIdAsync(int id);
    Task AddAsync(Contract contract);
    Task UpdateAsync(Contract contract);
    Task DeleteAsync(int id);

    Task<Car?> GetCarByPlateAsync(string plate);
    Task<Customer?> GetCustomerByFullNameAsync(string? first, string? middle, string? last);
}