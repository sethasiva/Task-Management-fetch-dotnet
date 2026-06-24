using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<User>>>> GetAllUsers()
    {
        var response = await _userService.GetAllUsersAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<User>>> GetUserById(int id)
    {
        var response = await _userService.GetUserByIdAsync(id);
        if (!response.Success)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    [HttpGet("{id}/tasks")]
    public async Task<ActionResult<ApiResponse<UserWithTasksDto>>> GetUserWithTasks(int id)
    {
        var response = await _userService.GetUserWithTasksAsync(id);
        if (!response.Success)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<User>>> AddUser([FromBody] CreateUserDto dto)
    {
        var response = await _userService.AddUserAsync(dto);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return CreatedAtAction(nameof(GetUserById), new { id = response.Data?.UserId }, response);
    }
}