using Microsoft.EntityFrameworkCore;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchQuery = null)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(c =>
                c.FirstName.Contains(searchQuery) || 
                (c.MiddleName != null && c.MiddleName.Contains(searchQuery)) ||
                c.LastName.Contains(searchQuery) ||
                c.PhoneNumber.Contains(searchQuery)); 
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Customer> GetByIdAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        return customer;
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, CustomerUpdateDto customerDto)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            throw new Exception("Customer not found");
        }

        customer.FirstName = customerDto.FirstName;
        customer.MiddleName = customerDto.MiddleName;
        customer.LastName = customerDto.LastName;
        customer.PhoneNumber = customerDto.PhoneNumber;
        customer.NationalityId = customerDto.NationalityId;
        customer.PassportImage = customerDto.PassportImage;
        customer.LicenseImage = customerDto.LicenseImage;

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name)
    {
        return await _context.Customers
            .Where(c => (c.FirstName + " " + c.MiddleName + " " + c.LastName).Contains(name))
            .ToListAsync();
    }

}
