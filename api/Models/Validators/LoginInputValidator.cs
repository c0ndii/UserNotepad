using FluentValidation;

namespace UserNotepad.Models.Validators
{
    public class LoginInputValidator : AbstractValidator<LoginInput>
    {
        public LoginInputValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!");
        }
    }
}
