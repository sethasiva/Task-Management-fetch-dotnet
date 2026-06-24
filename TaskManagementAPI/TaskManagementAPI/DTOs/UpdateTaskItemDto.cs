using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs;

public class UpdateTaskItemDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;

    public int UserId { get; set; }
}