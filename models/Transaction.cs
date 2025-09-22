using System;
using System.ComponentModel.DataAnnotations;


    public enum TransactionType
    {
        Income,
        Expense
    }

    //  public enum TransactionCategory
    // {
    //     // Income categories
    //     RentalPayment,
    //     Deposit,
    //     LateFee,
    //     DamageFee,
        
    //     // Expense categories
    //     Maintenance,
    //     Insurance,
    //     Fuel,
    //     Taxes,
    //     Salaries,
    //     Utilities,
    //     Marketing,
    //     OfficeCosts,
    //     VehiclePurchase,
    //     Other
    // }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        
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
        
        // Optional: Link to a contract if the transaction is related to a rental
        public int? ContractId { get; set; }
        public virtual Contract? Contract { get; set; }
        
        // Optional: Link to a car if the transaction is related to a specific vehicle
        public int? CarId { get; set; }
        public virtual Car? Car { get; set; }
        
        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
