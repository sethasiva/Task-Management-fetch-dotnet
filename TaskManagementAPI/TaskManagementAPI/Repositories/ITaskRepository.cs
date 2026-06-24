using TaskManagementAPI.Models;
using TaskManagementSystem.DTOs;


namespace TaskManagementSystem.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItemResponseDto>> GetAllTasksAsync();
    Task<TaskItemResponseDto?> GetTaskByIdAsync(int id);
    Task<List<TaskItemResponseDto>> SearchTasksByNameAsync(string name);
    Task<TaskItemResponseDto> AddTaskAsync(TaskItem task);

    Task ChangeStatusAsync(int id, string status);
    Task DeleteTaskAsync(int id);

    Task<bool> UserExistsAsync(int userId);

    bool UpdateTask(int id, UpdateTaskItemDto dto);





}