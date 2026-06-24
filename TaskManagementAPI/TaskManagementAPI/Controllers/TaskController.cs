using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TaskItemResponseDto>>>> GetAllTasks()
    {
        var response = await _taskService.GetAllTasksAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> GetTaskById(int id)
    {
        var response = await _taskService.GetTaskByIdAsync(id);
        if (!response.Success)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<TaskItemResponseDto>>>> SearchTasks([FromQuery] string? name)
    {
        var response = await _taskService.SearchTasksAsync(name);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> AddTask([FromBody] CreateTaskItemDto dto)
    {
        var response = await _taskService.AddTaskAsync(dto);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return CreatedAtAction(nameof(GetTaskById), new { id = response.Data?.TaskId }, response);
    }


    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
    {
        var response = await _taskService.ChangeStatusAsync(id, dto);
        if (!response.Success)
        {
            if (response.Errors.Any(e => e.Contains("does not exist")))
            {
                return NotFound(response);
            }
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTask(int id)
    {
        var response = await _taskService.DeleteTaskAsync(id);
        if (!response.Success)
        {
            if (response.Errors.Any(e => e.Contains("does not exist")))
            {
                return NotFound(response);
            }
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTask(
        int id,
        [FromBody] UpdateTaskItemDto dto)
    {
        var result = _taskService.UpdateTask(id, dto);

        if (!result.Success)
        {
            if (result.Message == "Task not found.")
                return NotFound(result);

            return BadRequest(result);
        }

        return Ok(result);
    }
}