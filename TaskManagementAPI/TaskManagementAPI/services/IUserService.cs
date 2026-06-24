using TaskManagementAPI.Models;
using TaskManagementSystem.DTOs;

namespace TaskManagementSystem.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<List<User>>> GetAllUsersAsync();
    Task<ApiResponse<User>> GetUserByIdAsync(int id);
    Task<ApiResponse<User>> AddUserAsync(CreateUserDto dto);
    Task<ApiResponse<UserWithTasksDto>> GetUserWithTasksAsync(int id);
}