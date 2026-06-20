using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs;

public class ChangeStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}