    using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/contracts")]
public class ContractController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet("fetch/all")]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery(Name = "search_query")] string? searchQuery = null)
    {
        var (contracts, totalCount) = await _contractService.GetAllContractsAsync(pageNumber, pageSize, searchQuery);
        return Ok(new { contracts, totalCount });
    }
    [HttpGet("available-cars")]
    // public async Task<ActionResult<IEnumerable<Car>>> GetAvailableCars()
    // {
    //     var availableCars = await _contractService.GetAvailableCarsAsync();
    //     return Ok(availableCars);
    // }


    [HttpGet("{id}")]
    public async Task<ActionResult<Contract>> GetContract(int id)
    {
        var contract = await _contractService.GetContractByIdAsync(id);
        return Ok(contract);
    }
    [HttpPost]
    public async Task<ActionResult<ContractResponseDto>> CreateContract([FromBody] ContractRequestDto dto)
    {
        try
        {
            var createdContract = await _contractService.CreateContractFromDtoAsync(dto);
            return Ok(createdContract);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Contract>> UpdateContract(int id, Contract contract)
    {
        if (id != contract.Cid)
        {
            return BadRequest("Contract ID mismatch.");
        }

        try
        {
            await _contractService.UpdateContractAsync(contract);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Contract with ID {id} not found.");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        try
        {
            await _contractService.DeleteContractAsync(id);
            return Ok(new
            {
                message = "Contract deleted successfully.",
                @class = "success-snackbar"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Contract with ID {id} not found.");
        }
    }
    // [HttpGet("car/{carId}")]
    // public async Task<ActionResult<IEnumerable<Contract>>> GetContractsByCarId(int carId)
    // {
    //     var contracts = await _contractService.GetContractsByCarIdAsync(carId);
    //     return Ok(contracts);
    // }
    // [HttpGet("customer/{customerId}")]
    // public async Task<ActionResult<IEnumerable<Contract>>> GetContractsByCustomerId(int customerId)
    // {
    //     var contracts = await _contractService.GetContractsByCustomerIdAsync(customerId);
    //     return Ok(contracts);
    // }


    [HttpPost("return/{id}")]
    public async Task<IActionResult> ReturnContract(int id, [FromBody] UpdateCheckInDateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _contractService.ReturnContract(id, dto.CheckInDate);
            return Ok(new { message = "Check-in date updated successfully", @class = "success-snackbar" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
        
    [HttpPost("extend/{id}")]
    public async Task<IActionResult> ExtendContract(int id, [FromBody] UpdateCheckInDateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _contractService.ExtendContract(id, dto.CheckInDate);
            return Ok(new { message = "Contract updated successfully", @class = "success-snackbar" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    }