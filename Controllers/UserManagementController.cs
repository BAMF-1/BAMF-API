using BAMF_API.Interfaces.UserInterfaces;
using BAMF_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BAMF_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]

    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public AdminUsersController(IUserService userService) => _userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _userService.GetAllUsersAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound("User not found") : Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User updatedUser)
        {
            await _userService.UpdateUserAsync(id, updatedUser);
            return Ok("User updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("User deleted");
        }
    }
}

// M.B