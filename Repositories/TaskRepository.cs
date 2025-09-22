using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;
    
    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<ToDoTask>> GetAllTasksAsync()
    {
        return await _context.Tasks
            .OrderByDescending(t => t.IsImportant)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ToDoTask>> GetActiveTasksAsync()
    {
        return await _context.Tasks
            .Where(t => t.Status == TaskStatus.Active || t.Status == TaskStatus.Overdue)
            .OrderByDescending(t => t.IsImportant)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ToDoTask>> GetCompletedTasksAsync()
    {
        return await _context.Tasks
            .Where(t => t.Status == TaskStatus.Completed)
            .OrderByDescending(t => t.CompletedAt)
            .ToListAsync();
    }
    
    public async Task<ToDoTask> GetTaskByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }
    
    public async Task<ToDoTask> CreateTaskAsync(ToDoTask task)
    {
        task.CreatedAt = DateTime.UtcNow;
        task.Status = TaskStatus.Active;
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        return task;
    }
    
    public async Task<bool> UpdateTaskAsync(ToDoTask task)
    {
        _context.Entry(task).State = EntityState.Modified;
        
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TaskExists(task.Id))
            {
                return false;
            }
            throw;
        }
    }
    
    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return false;
        }
        
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> CompleteTaskAsync(int id, string completedBy)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return false;
        }
        
        task.Status = TaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        task.CompletedBy = completedBy;
        
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<int> UpdateOverdueTasksAsync()
    {
        var today = DateTime.Today;
        var overdueTasks = await _context.Tasks
            .Where(t => t.Status == TaskStatus.Active && t.DueDate.Date < today)
            .ToListAsync();
            
        foreach (var task in overdueTasks)
        {
            task.Status = TaskStatus.Overdue;
        }
        
        return await _context.SaveChangesAsync();
    }
    
    private bool TaskExists(int id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }
}