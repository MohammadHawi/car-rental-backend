using System;
using System.ComponentModel.DataAnnotations;

public class TaskDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    public string? Status { get; set; }
    
    [Required]
    public bool IsImportant { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public string CompletedBy { get; set; }
}

public class CreateTaskDto
{
    [Required]
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    [Required]
    public bool IsImportant { get; set; }
    
    public string? CreatedBy { get; set; }
}

public class UpdateTaskDto
{
    [Required]
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    [Required]
    public bool IsImportant { get; set; }
}