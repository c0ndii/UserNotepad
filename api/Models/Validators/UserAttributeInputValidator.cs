using FluentValidation;

namespace UserNotepad.Models.Validators
{
    public class UserAttributeInputValidator : AbstractValidator<UserAttributeInput>
    {
        public UserAttributeInputValidator()
        {
            RuleFor(x => x.Key).NotEmpty().WithMessage("Key is required!");
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required!");
            RuleFor(x => x.ValueType).NotEmpty().WithMessage("Value type is required!");
        }
    }
}