
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


    [Route("api/transaction")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
        {
            var transactions = await _transactionRepository.GetAllTransactionsAsync();
            return Ok(transactions.Select(MapToDto));
        }

        // GET: api/transactions/income
        [HttpGet("income")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetIncomeTransactions()
        {
            var transactions = await _transactionRepository.GetTransactionsByTypeAsync(TransactionType.Income);
            return Ok(transactions.Select(MapToDto));
        }

        // GET: api/transactions/expenses
        [HttpGet("expenses")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetExpenseTransactions()
        {
            var transactions = await _transactionRepository.GetTransactionsByTypeAsync(TransactionType.Expense);
            return Ok(transactions.Select(MapToDto));
        }

        // GET: api/transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return MapToDto(transaction);
        }

        // GET: api/transactions/summary
        [HttpGet("summary")]
        public async Task<ActionResult<FinancialSummaryDto>> GetFinancialSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var totalIncome = await _transactionRepository.GetTotalIncomeAsync(start, end);
            var totalExpenses = await _transactionRepository.GetTotalExpensesAsync(start, end);
            var netIncome = totalIncome - totalExpenses;

            return new FinancialSummaryDto
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                NetIncome = netIncome,
                StartDate = start,
                EndDate = end
            };
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto createDto)
        {
            var transaction = new Transaction
            {
                Type = createDto.Type,
                Category = createDto.Category,
                Amount = createDto.Amount,
                Date = createDto.Date,
                Description = createDto.Description ?? string.Empty,
                ContractId = createDto.ContractId,
                CarId = createDto.CarId,
                CreatedBy = createDto.CreatedBy ?? User?.Identity?.Name ?? "Unknown"
            };

            await _transactionRepository.CreateTransactionAsync(transaction);

            return CreatedAtAction(
                nameof(GetTransaction),
                new { id = transaction.Id },
                MapToDto(transaction)
            );
        }

        // PUT: api/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, UpdateTransactionDto updateDto)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            // Cannot change the transaction type after creation
            transaction.Category = updateDto.Category;
            transaction.Amount = updateDto.Amount;
            transaction.Date = updateDto.Date;
            transaction.Description = updateDto.Description ?? string.Empty;
            transaction.ContractId = updateDto.ContractId;
            transaction.CarId = updateDto.CarId;
            transaction.UpdatedBy = updateDto.UpdatedBy ?? User?.Identity?.Name ?? "Unknown";

            await _transactionRepository.UpdateTransactionAsync(transaction);

            return NoContent();
        }

        // DELETE: api/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            await _transactionRepository.DeleteTransactionAsync(id);

            return NoContent();
        }

        // Helper method to map Transaction to TransactionDto
        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Type = transaction.Type,
                Category = transaction.Category,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description,
                ContractId = transaction.ContractId,
                ContractNumber = transaction.Contract?.Id.ToString(),
                CarId = transaction.CarId,
                CarDetails = transaction.Car != null ? $"{transaction.Car.Brand} {transaction.Car.Model}" : null
            };
        }
    }
