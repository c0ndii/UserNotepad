using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            this._authService = authService;
            this._logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterInput input, CancellationToken cancellationToken)
        {
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
    }
}
