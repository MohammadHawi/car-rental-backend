public class CarService : ICarService 
{
    private readonly ICarRepository _carRepository;

    public CarService(ICarRepository carRepository) 
    {
        _carRepository = carRepository;
    }
    public async Task<(IEnumerable<Car> Items, int TotalCount)> GetAllCarsAsync(int pageNumber, int pageSize, string? searchQuery = null) 
    {
        return await _carRepository.GetAllAsync(pageNumber, pageSize, searchQuery);
    }
    public async Task<Car> GetCarByIdAsync(int id) 
    {
        var car = await _carRepository.GetByIdAsync(id);
        if (car == null) 
        {
            throw new KeyNotFoundException("Car not found.");
        }
        return car;
    }

    public async Task AddCarAsync(Car car) 
    {
        await _carRepository.AddAsync(car);
    }

    public async Task UpdateCarAsync(Car car) 
    {
        await _carRepository.UpdateAsync(car);
    }

    public async Task DeleteCarAsync(int id) 
    {
        await _carRepository.DeleteAsync(id);
    }
}