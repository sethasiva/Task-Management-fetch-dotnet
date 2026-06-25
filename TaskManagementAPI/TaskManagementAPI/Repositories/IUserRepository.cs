using TaskManagementAPI.Models;
using TaskManagementSystem.DTOs;



public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> AddUserAsync(User user);
    Task<UserWithTasksDto?> GetUserWithTasksAsync(int id);
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(int id);
    Task<User> AddUser(CreateUserDto dto);
    Task<UserWithTasksDto> GetUserWithTasks(int id);
}