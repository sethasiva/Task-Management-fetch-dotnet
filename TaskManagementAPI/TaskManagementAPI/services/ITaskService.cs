using TaskManagementSystem.DTOs;

namespace TaskManagementSystem.Services.Interfaces;

public interface ITaskService
{
    Task<ApiResponse<List<TaskItemResponseDto>>> GetAllTasksAsync();
    Task<ApiResponse<TaskItemResponseDto>> GetTaskByIdAsync(int id);
    Task<ApiResponse<List<TaskItemResponseDto>>> SearchTasksAsync(string? name);
    Task<ApiResponse<TaskItemResponseDto>> AddTaskAsync(CreateTaskItemDto dto);
    Task<ApiResponse<TaskItemResponseDto>> UpdateTaskAsync(int id, UpdateTaskItemDto dto);
    Task<ApiResponse<object>> ChangeStatusAsync(int id, ChangeStatusDto dto);
    Task<ApiResponse<object>> DeleteTaskAsync(int id);
}