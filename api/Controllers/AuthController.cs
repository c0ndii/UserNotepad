using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using UserNotepad.Entities;
using UserNotepad.Models;
using UserNotepad.Services;

namespace UserNotepad.Controllers
{
    [Route("/api")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IHttpContextAccessor contextAccessor, ILogger<AuthController> logger)
        {
            this._authService = authService;
            this._contextAccessor = contextAccessor;
            this._logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterInput input, CancellationToken cancellationToken)
        {
            if (await _authService.IsUsernameTaken(input.Username, cancellationToken))
                return Conflict();

            await _authService.Register(input, cancellationToken);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginInput input, CancellationToken cancellationToken)
        {
            var result = await _authService.Login(input, cancellationToken);
            if (result is not null)
                return Ok(result);
            return Unauthorized();
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            string? username;

            try
            {
                username = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            } catch
            {
                return Unauthorized();
            }

            if (username == null)
                return Unauthorized();

            var result = await _authService.Me(username, cancellationToken);
            if (result is null)
                return Unauthorized();

            return Ok(result);
        }
    }
}
