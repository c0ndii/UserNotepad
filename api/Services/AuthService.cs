using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserNotepad.Entities;
using UserNotepad.Models;
using UserNotepad.Settings;

namespace UserNotepad.Services
{
    public interface IAuthService
    {
        public Task Register(RegisterInput input, CancellationToken cancellationToken);
        public Task<TokenDto> Login(LoginInput input, CancellationToken cancellationToken);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly Jwt _jwtSettings;
        private readonly IPasswordHasher<Operator> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, IOptions<Jwt> jwtSettings, IPasswordHasher<Operator> passwordHasher, ILogger<AuthService> logger)
        {
            this._context = context;
            this._jwtSettings = jwtSettings.Value;
            this._passwordHasher = passwordHasher;
            this._logger = logger;
        }

        public async Task<TokenDto?> Login(LoginInput input, CancellationToken cancellationToken)
        {
            var operatorLogin = await _context.Operators.SingleOrDefaultAsync(x => x.Username == input.Username, cancellationToken);
            if (operatorLogin is null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(operatorLogin, operatorLogin.PasswordHash, input.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            return GenerateJwtToken(operatorLogin.Username);
        }

        public async Task Register(RegisterInput input, CancellationToken cancellationToken)
        {
            var newOperator = new Operator
            {
                Nickname = input.Nickname,
                Username = input.Username,
            };

            newOperator.PasswordHash = _passwordHasher.HashPassword(newOperator, input.Password);

            await _context.Operators.AddAsync(newOperator, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private TokenDto GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new TokenDto
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Exiration = expires
            };
        }
    }
}
