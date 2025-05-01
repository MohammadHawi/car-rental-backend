public interface ICustomerRepository
{
    Task<(IEnumerable<Customer> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchQuery = null);
    Task<Customer> GetByIdAsync(int id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(int id,CustomerUpdateDto customer);
    Task DeleteAsync(int id);
    Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name);

}
