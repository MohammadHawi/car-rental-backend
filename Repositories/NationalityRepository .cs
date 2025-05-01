using Microsoft.EntityFrameworkCore;

public class NationalityRepository : INationalityRepository
{
    private readonly ApplicationDbContext _context;

    public NationalityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Nationality>> GetAllAsync()
    {
        return await _context.Nationality.ToListAsync();
    }
}
