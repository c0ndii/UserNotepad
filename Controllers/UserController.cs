using Microsoft.AspNetCore.Mvc;
using UserNotepad.Models;
using UserNotepad.Services;

namespace UserNotepad.Controllers
{
    [Route("/api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            this._userService = userService;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var user = await _userService.GetUser(id);

            if (user is not null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserInput user)
        {
            var createdUser = await _userService.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.ID }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserInput user)
        {
            var updatedUser = await _userService.UpdateUser(id, user);
            if (updatedUser is null)
                return NotFound();
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            if (await _userService.RemoveUser(id) is null)
                return NotFound();
            return NoContent();
        }
    }
}
