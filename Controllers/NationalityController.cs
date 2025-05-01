using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/Nationality")]
public class NationalityController : ControllerBase
{
    private readonly INationalityService _service;

    public NationalityController(INationalityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var nationalities = await _service.GetAllNationalitiesAsync();
        return Ok(nationalities);
    }
}
