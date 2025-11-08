using Microsoft.AspNetCore.Mvc;
using UserNotepad.Models;
using UserNotepad.Services;

namespace UserNotepad.Controllers
{
    [Route("/api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public UserController(IUserService userService, ILogger logger)
        {
            this._userService = userService;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserInput user)
        {
            await _userService.AddUser(user);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserInput user)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            return Ok();
        }
    }
}
