public interface ICarService
{
    Task<(IEnumerable<Car> Items, int TotalCount)> GetAllCarsAsync(int pageNumber, int pageSize, string? searchQuery = null);
    Task<Car> GetCarByIdAsync(int id);
    Task AddCarAsync(Car car);
    Task UpdateCarAsync(Car car);
    Task DeleteCarAsync(int id);
}