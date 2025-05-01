    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("fetch/all")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery(Name = "search_query")] string? searchQuery = null)
        {
            var (customers, totalCount) = await _customerService.GetAllCustomersAsync(pageNumber, pageSize, searchQuery);
            return Ok(new { customers, totalCount });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            try
            {
                await _customerService.AddCustomerAsync(customer);
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                // Check if the exception is related to a unique constraint violation
                if (ex.Message.Contains("IX_Customers_PhoneNumber"))
                {
                    return BadRequest(new
                    {
                        message = "Phone number is already in use. Please provide a different one.",
                        @class = "error-snackbar"  // You can customize this for styling
                    });
                }

                // For other exceptions
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred. Please try again later.",
                    @class = "error-snackbar"
                });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerUpdateDto customerDto)
        {
            await _customerService.UpdateCustomerAsync(id, customerDto);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id);

                return Ok(new
                {
                    message = "Customer deleted successfully!",
                    @class = "success-snackbar"
                });
            }
            catch (Exception ex)
            {
                // Optional: check if customer doesn't exist and return 404
                return BadRequest(new
                {
                    message = "Failed to delete customer.",
                    @class = "error-snackbar"
                });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Customer>>> SearchCustomers(string name)
        {
            var customers = await _customerService.SearchCustomersByNameAsync(name);
            return Ok(customers);
        }


    }
