public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetAllCustomersAsync(int pageNumber, int pageSize, string? searchQuery = null)
    {
        return await _customerRepository.GetAllAsync(pageNumber, pageSize, searchQuery);
    }

    public async Task<Customer> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }
        return customer;
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        await _customerRepository.AddAsync(customer);
    }

    public async Task UpdateCustomerAsync(int id, CustomerUpdateDto customer)
    {
        
        await _customerRepository.UpdateAsync(id,customer);
    }

    public async Task DeleteCustomerAsync(int id)
    {
        await _customerRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name)
    {
        return await _customerRepository.SearchCustomersByNameAsync(name);
    }


}
