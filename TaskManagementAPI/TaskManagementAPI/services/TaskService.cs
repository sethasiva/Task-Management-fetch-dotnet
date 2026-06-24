using System.Text.RegularExpressions;
using TaskManagementAPI.Models;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Repositories.Interfaces;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    private bool IsValidStatus(string status)
    {
        return status == "Todo" || status == "In Progress" || status == "Done";
    }

    public async Task<ApiResponse<List<TaskItemResponseDto>>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return new ApiResponse<List<TaskItemResponseDto>>
            {
                Success = true,
                Message = "Tasks retrieved successfully.",
                Data = tasks
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<TaskItemResponseDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving tasks.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<TaskItemResponseDto>> GetTaskByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return new ApiResponse<TaskItemResponseDto>
                {
                    Success = false,
                    Message = "Invalid task ID.",
                    Errors = new List<string> { "Task ID must be a positive integer." }
                };
            }

            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return new ApiResponse<TaskItemResponseDto>
                {
                    Success = false,
                    Message = "Task not found.",
                    Errors = new List<string> { $"Task with ID {id} does not exist." }
                };
            }

            return new ApiResponse<TaskItemResponseDto>
            {
                Success = true,
                Message = "Task retrieved successfully.",
                Data = task
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the task.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<TaskItemResponseDto>>> SearchTasksAsync(string? name)
    {
        try
        {
            var tasks = await _taskRepository.SearchTasksByNameAsync(name ?? string.Empty);
            return new ApiResponse<List<TaskItemResponseDto>>
            {
                Success = true,
                Message = "Search completed successfully.",
                Data = tasks
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<TaskItemResponseDto>>
            {
                Success = false,
                Message = "An error occurred while searching tasks.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<TaskItemResponseDto>> AddTaskAsync(CreateTaskItemDto dto)
    {
        var errors = new List<string>();

        // Validate Title
        if (string.IsNullOrWhiteSpace(dto.Title))
            errors.Add("Title is required.");
        else if (dto.Title.Length > 200)
            errors.Add("Title must not exceed 200 characters.");

        // Validate Description
        if (dto.Description != null && dto.Description.Length > 500)
            errors.Add("Description must not exceed 500 characters.");

        // Validate Status
        if (string.IsNullOrWhiteSpace(dto.Status) || !IsValidStatus(dto.Status))
            errors.Add("Status must be Todo, In Progress, or Done.");

        // Validate UserId
        if (dto.UserId <= 0)
            errors.Add("User ID is required.");
        else
        {
            var user = await _userRepository.GetUserByIdAsync(dto.UserId);
            if (user == null)
                errors.Add($"User with ID {dto.UserId} does not exist.");
        }

        if (errors.Any())
        {
            return new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = "Validation failed.",
                Errors = errors
            };
        }

        try
        {
            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                Status = dto.Status,
                UserId = dto.UserId,
                CreatedDate = DateTime.Now
            };

            var createdTask = await _taskRepository.AddTaskAsync(task);
            return new ApiResponse<TaskItemResponseDto>
            {
                Success = true,
                Message = "Task created successfully.",
                Data = createdTask
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = "An error occurred while creating the task.",
                Errors = new List<string> { ex.Message }
            };
        }
    }


    public async Task<ApiResponse<object>> ChangeStatusAsync(int id, ChangeStatusDto dto)
    {
        var errors = new List<string>();

        if (id <= 0)
            errors.Add("Invalid task ID.");

        if (string.IsNullOrWhiteSpace(dto.Status) || !IsValidStatus(dto.Status))
            errors.Add("Status must be Todo, In Progress, or Done.");

        if (errors.Any())
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed.",
                Errors = errors
            };
        }

        try
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Task not found.",
                    Errors = new List<string> { $"Task with ID {id} does not exist." }
                };
            }

            await _taskRepository.ChangeStatusAsync(id, dto.Status);
            return new ApiResponse<object>
            {
                Success = true,
                Message = $"Task status changed to '{dto.Status}' successfully.",
                Data = new { TaskId = id, NewStatus = dto.Status }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while changing the task status.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<object>> DeleteTaskAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid task ID.",
                    Errors = new List<string> { "Task ID must be a positive integer." }
                };
            }

            var existingTask = await _taskRepository.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Task not found.",
                    Errors = new List<string> { $"Task with ID {id} does not exist." }
                };
            }

            await _taskRepository.DeleteTaskAsync(id);
            return new ApiResponse<object>
            {
                Success = true,
                Message = "Task deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the task.",
                Errors = new List<string> { ex.Message }
            };
        }

    }

    public ApiResponse<string> UpdateTask(int id, UpdateTaskItemDto dto)
    {
        var errors = new List<string>();

        if (id <= 0)
            errors.Add("Invalid Task Id");

        if (string.IsNullOrWhiteSpace(dto.Title))
            errors.Add("Title is required");

        if (string.IsNullOrWhiteSpace(dto.Status))
            errors.Add("Status is required");

        if (dto.UserId <= 0)
            errors.Add("User Id is required");

        if (errors.Any())
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Validation Failed",
                Errors = errors
            };
        }

        var task = _taskRepository.GetTaskByIdAsync(id).Result;

        if (task == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Task not found."
            };
        }

        bool userExists = _taskRepository.UserExistsAsync(dto.UserId).Result;

        if (!userExists)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "User not found."
            };
        }

        bool updated = _taskRepository.UpdateTask(id, dto);

        if (!updated)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Update failed."
            };
        }

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Task Updated Successfully",
            Data = "Success"
        };
    }

}



        


    
