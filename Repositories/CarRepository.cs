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
        var query = _context.Cars.AsQueryable();

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
}