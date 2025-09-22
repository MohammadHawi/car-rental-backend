
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Contract)
                .Include(t => t.Car)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type)
        {
            return await _context.Transactions
                .Include(t => t.Contract)
                .Include(t => t.Car)
                .Where(t => t.Type == type)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Include(t => t.Contract)
                .Include(t => t.Car)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Contract)
                .Include(t => t.Car)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                throw new InvalidOperationException($"Transaction with ID {id} not found.");
            }

            return transaction;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            transaction.UpdatedAt = DateTime.UtcNow;
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions.Where(t => t.Type == TransactionType.Income);
            
            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
                
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);
                
            return await query.SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalExpensesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions.Where(t => t.Type == TransactionType.Expense);
            
            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
                
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);
                
            return await query.SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetNetIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var income = await GetTotalIncomeAsync(startDate, endDate);
            var expenses = await GetTotalExpensesAsync(startDate, endDate);
            return income - expenses;
        }
    }
