using Microsoft.EntityFrameworkCore;

public class ContractRepository : IContractRepository
{
    private readonly ApplicationDbContext _context;

    public ContractRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<ContractResponseDto> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchQuery = null)
    {
        var now = DateTime.Now;

        var today = DateOnly.FromDateTime(DateTime.Today);

        // Contracts ending today
        var contractsEndingToday = await _context.Contracts
            .Where(c => c.Returned == 0 &&
                        c.Status == 1 &&
                        c.CheckIn.HasValue &&
                        c.CheckIn.Value == today)
            .ToListAsync();

        // Contracts overdue
        var overdueContracts = await _context.Contracts
            .Where(c => c.Returned == 0 &&
                        (c.Status == 1 || c.Status == 3)  &&
                        c.CheckIn.HasValue &&
                        c.CheckIn.Value < today)
            .ToListAsync();


        // Update contracts ending today
        if (contractsEndingToday.Any())
        {
            foreach (var contract in contractsEndingToday)
            {
                contract.Status = 3; // Mark as ending today
            }
        }

        // Update overdue contracts
        if (overdueContracts.Any())
        {
            foreach (var contract in overdueContracts)
            {
                contract.Status = 2; // Mark as overdue
            }
        }

        // Save all changes at once
        if (contractsEndingToday.Any() || overdueContracts.Any())
        {
            await _context.SaveChangesAsync();
        }

        // Now proceed with the normal query
        var query = _context.Contracts
            .Include(c => c.Car)
            .Include(c => c.Customer)
            .Where(c => c.Returned == 0)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(c =>
                (c.Customer.FirstName + " " + c.Customer.MiddleName + " " + c.Customer.LastName).Contains(searchQuery) ||
                c.Car.Plate.ToString().Contains(searchQuery));
        }

        var totalCount = await query.CountAsync();

        var rawItems = await query
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
                PhoneNumber = contract.Customer != null ? contract.Customer.PhoneNumber : null,
                Status = contract.Status, // Now this will be correct since we updated it above
                Total = (contract.CheckIn.HasValue && contract.CheckOut.HasValue)
                    ? (int)(contract.Price * (contract.CheckIn.Value.DayNumber - contract.CheckOut.Value.DayNumber))
                    : 0,
                Balance = (contract.CheckIn.HasValue && contract.CheckOut.HasValue)
                    ? (int)(contract.Price * (contract.CheckIn.Value.DayNumber - contract.CheckOut.Value.DayNumber)
                        - (contract.Paid ?? 0))
                    : 0,
            })
            .ToListAsync();

        var sortedItems = rawItems
            .OrderBy(c => c.Status)
            .ThenBy(c => c.CheckIn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (sortedItems, totalCount);
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
            // _context.Transactions.Remove(contract.Transactions);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Car?> GetCarByPlateAsync(int plate)
    {
        if (int.TryParse(plate.ToString(), out int plateNumber))
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