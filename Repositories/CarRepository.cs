using Microsoft.EntityFrameworkCore;

public class CarRepository : ICarRepository
{
    private readonly ApplicationDbContext _context;

    public CarRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Car> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchQuery = null)
    {
        var query = _context.Cars
            // .Include(c => c.CarStatus) // Include status data
            .AsQueryable();


        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(c => c.Plate.ToString().Contains(searchQuery));
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<Car> GetByIdAsync(int id)
    {
        return await _context.Cars.FindAsync(id) ?? throw new InvalidOperationException($"Car with ID {id} not found.");
    }

    public async Task AddAsync(Car car)
    {
        await _context.Cars.AddAsync(car);
        await _context.SaveChangesAsync();

        // Create initial status record (default: available)
        var carStatus = new CarStatus
        {
            CarId = car.Id,
            Status = 0 // Available
        };

        await _context.CarStatuses.AddAsync(carStatus);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateAsync(Car car)
    {
        _context.Cars.Update(car);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var car = await GetByIdAsync(id);
        if (car != null)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
    }

    // New methods for handling car status
    public async Task UpdateCarStatusAsync(int carId, int status)
    {
        var carStatus = await _context.CarStatuses.FirstOrDefaultAsync(cs => cs.CarId == carId);

        if (carStatus == null)
        {
            // Create new status record if doesn't exist
            carStatus = new CarStatus
            {
                CarId = carId,
                Status = status
            };
            await _context.CarStatuses.AddAsync(carStatus);
        }
        else
        {
            carStatus.Status = status;
            carStatus.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int?> GetCarStatusAsync(int carId)
    {
        var carStatus = await _context.CarStatuses.FirstOrDefaultAsync(cs => cs.CarId == carId);
        return carStatus?.Status;
    }
    
     public async Task<CarHistoryDto> GetCarHistoryAsync(int carId)
    {
        // Fetch the car
        var car = await _context.Cars.FindAsync(carId);
        if (car == null)
            throw new InvalidOperationException($"Car with ID {carId} not found.");

        // Fetch contracts
        var contracts = await _context.Contracts
            .Include(c => c.Customer)
            .Where(c => c.CarId == carId)
            .OrderByDescending(c => c.CheckOut)
            .ToListAsync();

        // Fetch transactions
        var transactions = await _context.Transactions
            .Include(t => t.Contract)
            .Include(t => t.Car)
            .Where(t => t.CarId == carId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        // Calculate summary
        var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var summary = new FinancialSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetIncome = totalIncome - totalExpenses,
            StartDate = transactions.Any() ? transactions.Min(t => t.Date) : DateTime.MinValue,
            EndDate = transactions.Any() ? transactions.Max(t => t.Date) : DateTime.MinValue
        };

        // Build DTO
        return new CarHistoryDto
        {
            CarId = car.Id,
            CarDetails = $"{car.Brand} {car.Model} - {car.Plate}",
            Contracts = contracts.Select(c => new ContractResponseDto
            {
                Id = c.Id,
                CustomerName = $"{c.Customer.FirstName} {c.Customer.LastName}",
                Price = c.Price,
                CheckIn = c.CheckIn,
                CheckOut = c.CheckOut,
                Status = c.Status,
                PhoneNumber = c.Customer.PhoneNumber
            }),

            Transactions = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                Type = t.Type,
                Category = t.Category,
                Amount = t.Amount,
                Date = t.Date,
                Description = t.Description,
                ContractId = t.ContractId,
                ContractNumber = t.Contract?.Id.ToString(),
                CarId = t.CarId,
                CarDetails = t.Car != null ? $"{t.Car.Brand} {t.Car.Model}" : null
            }),
            Summary = summary
        };
    }
}