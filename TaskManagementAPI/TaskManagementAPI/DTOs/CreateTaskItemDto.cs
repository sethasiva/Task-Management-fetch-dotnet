using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs;

public class CreateTaskItemDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public string Status { get; set; } = "Todo";

    [Required]
    public int UserId { get; set; }
}