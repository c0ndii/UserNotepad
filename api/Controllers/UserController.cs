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
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            this._userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken, 
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _userService.GetAllUsers(new PageInput
                {
                    Page = page,
                    PageSize = pageSize,
                }, cancellationToken));
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Error when getting all users!");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUser(id, cancellationToken);

                if (user is not null)
                {
                    return Ok(user);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when getting user with id={id}!");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserInput user, CancellationToken cancellationToken)
        {
            try
            {
                var createdUser = await _userService.AddUser(user, cancellationToken);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.ID }, createdUser);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding new user!");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserInput user, CancellationToken cancellationToken)
        {
            try
            {
                var updatedUser = await _userService.UpdateUser(id, user, cancellationToken);
                if (updatedUser is null)
                    return NotFound();
                return Ok(updatedUser);
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when updating user with id={id}!");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                if (await _userService.RemoveUser(id, cancellationToken) is null)
                    return NotFound();
                return NoContent();
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when deleting user with id={id}!");
                return StatusCode(500, "Internal server error occurred");
            } 
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetReport(CancellationToken cancellationToken)
        {
            try
            {
                var generationDateTime = DateTime.UtcNow;

                var report = await _userService.GetReport(generationDateTime, cancellationToken);

                var fileName = $"{generationDateTime:dd.MM.yyyy_HH-mm-ss}.pdf";

                Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
                Response.Headers["Access-Control-Expose-Headers"] = "Content-Disposition";

                return File(report, "application/pdf");
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error when generating report!");
                return StatusCode(500, "Internal server error occurred");
            }
        }
    }
}
