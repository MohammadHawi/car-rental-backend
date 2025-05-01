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
    }