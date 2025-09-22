using System;
using System.ComponentModel.DataAnnotations;


    public class TransactionDto
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public int Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public int? ContractId { get; set; }
        public string? ContractNumber { get; set; }
        public int? CarId { get; set; }
        public string? CarDetails { get; set; }
    }

    public class CreateTransactionDto
    {
        [Required]
        public TransactionType Type { get; set; }
        
        [Required]
        public int Category { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int? ContractId { get; set; }
        public int? CarId { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateTransactionDto
    {
        [Required]
        public int Category { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int? ContractId { get; set; }
        public int? CarId { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class FinancialSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
