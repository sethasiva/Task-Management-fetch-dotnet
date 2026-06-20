using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.services
{
    public interface IUserService
    {
        ApiResponse<User> AddUser(CreateUserDto dto);
    }
}
