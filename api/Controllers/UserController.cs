using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserNotepad.Models;
using UserNotepad.Services;

namespace UserNotepad.Controllers
{
    [Route("/api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken, 
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _userService.GetAllUsers(new PageInput
            {
                Page = page,
                PageSize = pageSize,
            }, cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUser(id, cancellationToken);

            if (user is not null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserInput user, CancellationToken cancellationToken)
        {
            var createdUser = await _userService.AddUser(user, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.ID }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserInput user, CancellationToken cancellationToken)
        {
            var updatedUser = await _userService.UpdateUser(id, user, cancellationToken);
            if (updatedUser is null)
                return NotFound();
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            if (await _userService.RemoveUser(id, cancellationToken) is null)
                return NotFound();
            return NoContent();
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetReport(CancellationToken cancellationToken)
        {
            var generationDateTime = DateTime.UtcNow;

            var report = await _userService.GetReport(generationDateTime, cancellationToken);

            var fileName = $"{generationDateTime:dd-MM-yyyy_HH-mm-ss}.pdf";

            Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
            Response.Headers["Access-Control-Expose-Headers"] = "Content-Disposition";

            return File(report, "application/pdf");
        }
    }
}
