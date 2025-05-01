public interface ICarRepository
{
    Task<(IEnumerable<Car> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchQuery = null);
    Task<Car> GetByIdAsync(int id);
    Task AddAsync(Car car);
    Task UpdateAsync(Car car);
    Task DeleteAsync(int id);
}