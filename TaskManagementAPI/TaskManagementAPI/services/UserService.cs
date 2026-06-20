using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;
using TaskManagementAPI.services;

namespace TaskManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ApiResponse<List<User>> GetAllUsers()
        {
            try
            {
                var users = _userRepository.GetAllUsers();
                return new ApiResponse<List<User>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = users
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<User>>
                {
                    Success = false,
                    Message = "Error retrieving users",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public ApiResponse<User> AddUser(CreateUserDto dto)
        {
            var errors = new List<string>();

            // Validation
            if (string.IsNullOrWhiteSpace(dto.UserName))
                errors.Add("UserName is required");

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required");
            else if (!IsValidEmail(dto.Email))
                errors.Add("Invalid email format");

            if (errors.Any())
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };
            }

            try
            {
                var user =
