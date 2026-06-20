using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.services;

namespace TaskManagementAPI.Controllers
{
  
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var result = _service.GetAllUsers();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddUser(CreateUserDto dto)
        {
            var result = _service.AddUser(dto);

            return Created("", result);
        }
    }
}
