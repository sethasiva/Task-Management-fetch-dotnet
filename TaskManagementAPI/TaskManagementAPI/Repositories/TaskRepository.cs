using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using TaskManagementAPI.Models;
using TaskManagementSystem.DTOs;

using TaskManagementSystem.Repositories.Interfaces;

namespace TaskManagementSystem.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly string _connectionString;

    public TaskRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<List<TaskItemResponseDto>> GetAllTasksAsync()
    {
        var tasks = new List<TaskItemResponseDto>();
        var sql = @"
            SELECT 
                t.TaskId, 
                t.Title, 
                t.Description, 
                t.Status, 
                t.CreatedDate,
                t.UserId, 
                u.UserName
            FROM Tasks t
            INNER JOIN Users u ON t.UserId = u.UserId
            ORDER BY t.CreatedDate DESC";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
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
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserName = reader.GetString(reader.GetOrdinal("UserName"))
            });
        }

        return tasks;
    }

    public async Task<TaskItemResponseDto?> GetTaskByIdAsync(int id)
    {
        var sql = @"
            SELECT 
                t.TaskId, 
                t.Title, 
                t.Description, 
                t.Status, 
                t.CreatedDate,
                t.UserId, 
                u.UserName
            FROM Tasks t
            INNER JOIN Users u ON t.UserId = u.UserId
            WHERE t.TaskId = @TaskId";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TaskId", id);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new TaskItemResponseDto
            {
                TaskId = reader.GetInt32(reader.GetOrdinal("TaskId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserName = reader.GetString(reader.GetOrdinal("UserName"))
            };
        }

        return null;
    }

    public async Task<List<TaskItemResponseDto>> SearchTasksByNameAsync(string name)
    {
        var tasks = new List<TaskItemResponseDto>();
        var sql = @"
            SELECT 
                t.TaskId, 
                t.Title, 
                t.Description, 
                t.Status, 
                t.CreatedDate,
                t.UserId, 
                u.UserName
            FROM Tasks t
            INNER JOIN Users u ON t.UserId = u.UserId
            WHERE t.Title LIKE @Name
            ORDER BY t.CreatedDate DESC";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", "%" + name + "%");

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
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
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                UserName = reader.GetString(reader.GetOrdinal("UserName"))
            });
        }

        return tasks;
    }

    public async Task<TaskItemResponseDto> AddTaskAsync(TaskItem task)
    {
        var sql = @"
            INSERT INTO Tasks (Title, Description, Status, CreatedDate, UserId)
            OUTPUT INSERTED.TaskId
            VALUES (@Title, @Description, @Status, @CreatedDate, @UserId)";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Title", task.Title);
        command.Parameters.AddWithValue("@Description", (object?)task.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", task.Status);
        command.Parameters.AddWithValue("@CreatedDate", task.CreatedDate);
        command.Parameters.AddWithValue("@UserId", task.UserId);

        await connection.OpenAsync();
        var taskId = (int)await command.ExecuteScalarAsync();

        // Get the full task with user name
        var createdTask = await GetTaskByIdAsync(taskId);
        return createdTask ?? throw new Exception("Failed to retrieve created task.");
    }

  

    public async Task ChangeStatusAsync(int id, string status)
    {
        var sql = "UPDATE Tasks SET Status = @Status WHERE TaskId = @TaskId";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TaskId", id);
        command.Parameters.AddWithValue("@Status", status);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteTaskAsync(int id)
    {
        var sql = "DELETE FROM Tasks WHERE TaskId = @TaskId";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TaskId", id);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public bool UpdateTask(int id, UpdateTaskItemDto dto)
    {
        using SqlConnection con = new SqlConnection(_connectionString);

        string sql = @"
        UPDATE Tasks
        SET Title = @Title,
            Description = @Description,
            Status = @Status,
            UserId = @UserId
        WHERE TaskId = @TaskId";

        using SqlCommand cmd = new SqlCommand(sql, con);

        cmd.Parameters.AddWithValue("@Title", dto.Title);
        cmd.Parameters.AddWithValue("@Description",
            (object?)dto.Description ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@Status", dto.Status);
        cmd.Parameters.AddWithValue("@UserId", dto.UserId);
        cmd.Parameters.AddWithValue("@TaskId", id);

        con.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        string sql = "SELECT COUNT(*) FROM Users WHERE UserId = @UserId";

        using SqlConnection con = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(sql, con);

        cmd.Parameters.AddWithValue("@UserId", userId);

        await con.OpenAsync();

        int count = (int)await cmd.ExecuteScalarAsync();

        return count > 0;
    }




}