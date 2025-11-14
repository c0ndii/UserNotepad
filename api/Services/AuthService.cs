using Microsoft.AspNetCore.Identity;
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
        public Task<LoginDto?> Login(LoginInput input, CancellationToken cancellationToken);
        public Task<OperatorDto?> Me(string username, CancellationToken cancellationToken);
        public Task<bool> IsUsernameTaken(string username, CancellationToken cancellationToken);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly Jwt _jwtSettings;
        private readonly IPasswordHasher<Operator> _passwordHasher;

        public AuthService(AppDbContext context, IOptions<Jwt> jwtSettings, IPasswordHasher<Operator> passwordHasher)
        {
            this._context = context;
            this._jwtSettings = jwtSettings.Value;
            this._passwordHasher = passwordHasher;
        }

        public async Task<LoginDto?> Login(LoginInput input, CancellationToken cancellationToken)
        {
            var operatorLogin = await _context.Operators
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Username == input.Username, cancellationToken);
            if (operatorLogin is null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(operatorLogin, operatorLogin.PasswordHash, input.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            return GenerateJwtToken(operatorLogin);
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

        public async Task<OperatorDto?> Me(string username, CancellationToken cancellationToken)
        {
            var exists = await _context.Operators.SingleOrDefaultAsync(x => x.Username == username, cancellationToken);
            if (exists is not null)
                return new OperatorDto { Nickname = exists.Nickname };

            return null;
        }

        public async Task<bool> IsUsernameTaken(string username, CancellationToken cancellationToken)
        {
            return await _context.Operators.AnyAsync(x => x.Username == username, cancellationToken);
        }

        private LoginDto GenerateJwtToken(Operator op)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, op.Username),
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

            return new LoginDto
            {
                UserNickname = op.Nickname,
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                JwtExpiration = expires
            };
        }
    }
}
