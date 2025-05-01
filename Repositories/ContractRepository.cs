using Microsoft.EntityFrameworkCore;

public class ContractRepository : IContractRepository
{
    private readonly ApplicationDbContext _context;

    public ContractRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<ContractResponseDto> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchQuery = null)
    {
        var query = _context.Contracts
        .Include(c => c.Car)
        .Include(c => c.Customer)
        .Where(c => c.Returned == 0)
        .AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(c => c.Cid.ToString().Contains(searchQuery) || c.CarId.ToString().Contains(searchQuery));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(contract => new ContractResponseDto
            {
                Id = contract.Id,
                CarId = contract.CarId,
                Cid = contract.Cid,
                CarPlate = contract.Car != null ? contract.Car.Plate.ToString() : null,
                CustomerName = (contract.Customer != null 
                    ? $"{contract.Customer.FirstName} {contract.Customer.MiddleName} {contract.Customer.LastName}" 
                    : string.Empty).Trim(),
                Price = contract.Price,
                CheckIn = contract.CheckIn,
                CheckOut = contract.CheckOut,
                Deposit = contract.Deposit,
                Paid = contract.Paid,
                PhoneNumber = contract.Customer != null ? contract.Customer.PhoneNumber : null  ,
                Status = (contract.CheckIn.HasValue && DateTime.Now > contract.CheckIn.Value.ToDateTime(TimeOnly.MinValue))
                ? 2 // Overdue
                : contract.Status,
                Total = (contract.CheckIn.HasValue && contract.CheckOut.HasValue)
                    ? (int)(contract.Price * (contract.CheckIn.Value.DayNumber - contract.CheckOut.Value.DayNumber))
                    : 0,
                    
                Balance = (contract.CheckIn.HasValue && contract.CheckOut.HasValue)
                    ? (int)(contract.Price * (contract.CheckIn.Value.DayNumber - contract.CheckOut.Value.DayNumber) - contract.Paid ?? 0)
                    : 0,


            })
            .ToListAsync();

        return (items, totalCount);
    }


    public async Task<Contract> GetByIdAsync(int id)
    {
        return await _context.Contracts.FindAsync(id) ?? throw new InvalidOperationException($"Contract with ID {id} not found.");
    }

    public async Task AddAsync(Contract contract)
    {
        await _context.Contracts.AddAsync(contract);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Contract contract)
    {
        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var contract = await GetByIdAsync(id);
        if (contract != null)
        {
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Car?> GetCarByPlateAsync(string plate)
    {
        if (int.TryParse(plate, out int plateNumber))
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.Plate == plateNumber);
        }
        return null;
    }

    public async Task<Customer?> GetCustomerByFullNameAsync(string? first, string? middle, string? last)
    {
        return await _context.Customers.FirstOrDefaultAsync(c =>
            c.FirstName == first &&
            c.LastName == last &&
            (middle == null || c.MiddleName == middle));
    }

    

}