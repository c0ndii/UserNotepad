using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserNotepad.Entities;

namespace UserNotepad.Models.Validators
{
    public class RegisterInputValidator : AbstractValidator<RegisterInput>
    {
        public RegisterInputValidator(AppDbContext _context)
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required!")
                .MustAsync(async (username, cancellation) =>
                 {
                     return !await _context.Operators.AnyAsync(x => x.Username == username, cancellation);
                }).WithMessage("Username already taken!").WithErrorCode("409");
            RuleFor(x => x.Nickname).NotEmpty().WithMessage("Nickname is required!");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required!")
                .MinimumLength(8).WithMessage("Password must contain at least 8 characters!");
            RuleFor(x => x.RepeatPassword).Equal(x => x.Password).WithMessage("Confirm password differs from password!");
        }
    }
}
