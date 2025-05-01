public interface ICustomerService
{
    Task<(IEnumerable<Customer> Items, int TotalCount)> GetAllCustomersAsync(int pageNumber, int pageSize, string? searchQuery = null);
    Task<Customer> GetCustomerByIdAsync(int id);
    Task AddCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(int id, CustomerUpdateDto customer);
    Task DeleteCustomerAsync(int id);
    Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name);

}
