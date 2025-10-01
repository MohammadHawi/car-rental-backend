    using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/cars")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet("fetch/all")]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery(Name = "search_query")] string? searchQuery = null)
    {
        var (cars, totalCount) = await _carService.GetAllCarsAsync(pageNumber, pageSize, searchQuery);
        return Ok(new { cars, totalCount });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCarById(int id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null)
        {
            return NotFound();
        }
        return Ok(car);
    }
        
        // GET: api/cars/{carId}/history
    [HttpGet("/api/cars/{carId}/history")]
    public async Task<ActionResult<CarHistoryDto>> GetCarHistory(int carId)
    {
        try
        {
            var history = await _carService.GetCarHistoryAsync(carId);
            return Ok(history);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }


    }