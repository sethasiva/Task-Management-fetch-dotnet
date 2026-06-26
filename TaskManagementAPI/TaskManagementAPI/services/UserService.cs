

using TaskManagementAPI.Models;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Services.Interfaces;
namespace TaskManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllUsersAsync();

            return new ApiResponse<List<User>>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = users
            };
        }

        public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "Invalid user id."
                };
            }

            var user = await _repository.GetUserByIdAsync(id);

            if (user == null)
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            return new ApiResponse<User>
            {
                Success = true,
                Message = "User retrieved successfully.",
                Data = user
            };
        }

        public async Task<ApiResponse<User>> AddUserAsync(CreateUserDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.UserName))
                errors.Add("UserName is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required.");

            if (errors.Any())
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = errors
                };
            }

            // Check for duplicate email
            var existing = await _repository.GetUserByEmailAsync(dto.Email);
            if (existing != null)
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "Email already exists.",
                    Errors = new List<string> { "A user with this email already exists." }
                };
            }

            var user = await _repository.AddUserAsync(new User
            {
                UserName = dto.UserName,
                Email = dto.Email
            });

            return new ApiResponse<User>
            {
                Success = true,
                Message = "User created successfully.",
                Data = user
            };
        }

        public async Task<ApiResponse<UserWithTasksDto>> GetUserWithTasksAsync(int id)
        {
            var user = await _repository.GetUserWithTasksAsync(id);

            if (user == null)
            {
                return new ApiResponse<UserWithTasksDto>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            return new ApiResponse<UserWithTasksDto>
            {
                Success = true,
                Message = "User details retrieved successfully.",
                Data = user
            };
        }
    }
}