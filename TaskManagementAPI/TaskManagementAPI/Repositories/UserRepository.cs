using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using TaskManagementAPI.Models;
using TaskManagementSystem.DTOs;

using TaskManagementSystem.Repositories.Interfaces;

namespace TaskManagementSystem.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = new List<User>();
        var sql = "SELECT UserId, UserName, Email FROM Users ORDER BY UserName";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                Email = reader.GetString(reader.GetOrdinal("Email"))
            });
        }

        return users;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var sql = "SELECT UserId, UserName, Email FROM Users WHERE UserId = @UserId";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", id);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                Email = reader.GetString(reader.GetOrdinal("Email"))
            };
        }

        return null;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var sql = "SELECT UserId, UserName, Email FROM Users WHERE Email = @Email";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                Email = reader.GetString(reader.GetOrdinal("Email"))
            };
        }

        return null;
    }

    public async Task<User> AddUserAsync(User user)
    {
        var sql = @"
            INSERT INTO Users (UserName, Email)
            OUTPUT INSERTED.UserId
            VALUES (@UserName, @Email)";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserName", user.UserName);
        command.Parameters.AddWithValue("@Email", user.Email);

        await connection.OpenAsync();
        var userId = (int)await command.ExecuteScalarAsync();

        return new User
        {
            UserId = userId,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<UserWithTasksDto?> GetUserWithTasksAsync(int id)
    {
        var sql = @"
            SELECT 
                u.UserId, u.UserName, u.Email,
                t.TaskId, t.Title, t.Description, t.Status, t.CreatedDate, t.UserId as TaskUserId
            FROM Users u
            LEFT JOIN Tasks t ON u.UserId = t.UserId
            WHERE u.UserId = @UserId
            ORDER BY t.CreatedDate DESC";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", id);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        UserWithTasksDto? result = null;
        var tasks = new List<TaskItemResponseDto>();

        while (await reader.ReadAsync())
        {
            if (result == null)
            {
                result = new UserWithTasksDto
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Tasks = new List<TaskItemResponseDto>()
                };
            }

            // Check if task exists (not null)
            if (!reader.IsDBNull(reader.GetOrdinal("TaskId")))
            {
                tasks.Add(new TaskItemResponseDto
                {
                    TaskId = reader.GetInt32(reader.GetOrdinal("TaskId")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Description")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserId = reader.GetInt32(reader.GetOrdinal("TaskUserId")),
                    UserName = result.UserName
                });
            }
        }

        if (result != null)
        {
            result.Tasks = tasks;
        }

        return result;
    }

    public Task<List<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User> AddUser(CreateUserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<UserWithTasksDto> GetUserWithTasks(int id)
    {
        throw new NotImplementedException();
    }
}