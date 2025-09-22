using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ToDoTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    [Required]
    public TaskStatus Status { get; set; }
    
    [Required]
    public bool IsImportant { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? CreatedBy { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public string? CompletedBy { get; set; }
}

public enum TaskStatus
{
    Active,
    Overdue,
    Completed
}