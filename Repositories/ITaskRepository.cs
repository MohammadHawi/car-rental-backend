
public interface ITaskRepository
{
    Task<IEnumerable<ToDoTask>> GetAllTasksAsync();
    Task<IEnumerable<ToDoTask>> GetActiveTasksAsync();
    Task<IEnumerable<ToDoTask>> GetCompletedTasksAsync();
    Task<ToDoTask> GetTaskByIdAsync(int id);
    Task<ToDoTask> CreateTaskAsync(ToDoTask task);
    Task<bool> UpdateTaskAsync(ToDoTask task);
    Task<bool> DeleteTaskAsync(int id);
    Task<bool> CompleteTaskAsync(int id, string completedBy);
    Task<int> UpdateOverdueTasksAsync();
}