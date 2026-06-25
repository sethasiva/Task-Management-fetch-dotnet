
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
        public async Task<ApiResponse<User>> GetUserById(int id)
        {
            if (id <= 0)
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "Invalid user id."
                };
            }

            var user = await _repository.GetUserById(id);

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

        public async Task<ApiResponse<User>> AddUser(CreateUserDto dto)
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

            var user = await _repository.AddUser(dto);

            return new ApiResponse<User>
            {
                Success = true,
                Message = "User created successfully.",
                Data = user
            };
        }

        public async Task<ApiResponse<UserWithTasksDto>> GetUserWithTasks(int id)
        {
            var user = await _repository.GetUserWithTasks(id);

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

        public Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<User>> AddUserAsync(CreateUserDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<UserWithTasksDto>> GetUserWithTasksAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

