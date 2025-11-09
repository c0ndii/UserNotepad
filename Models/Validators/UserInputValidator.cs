using FluentValidation;

namespace UserNotepad.Models.Validators
{
    public class UserInputValidator : AbstractValidator<UserInput>
    {
        public UserInputValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required!")
                .MaximumLength(50).WithMessage("Name must not exceed length of 50!")
                .Matches(@"^[A-Za-z]+$").WithMessage("Name can contain only letters!");
            RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname is required!")
                .MaximumLength(150).WithMessage("Surname must not exceed length of 150!")
                .Matches(@"^[A-Za-z]+$").WithMessage("Surname can contain only letters!");
            RuleFor(x => x.BirthDate).NotEmpty().WithMessage("Birth date is required!")
                .LessThan(DateTime.Now).WithMessage("Birth date can not be set in future!");
            RuleFor(x => x.Attributes).Must(HaveUniqueKeys).WithMessage("Key value of attribute can not be duplicated!");
        }

        private bool HaveUniqueKeys(IEnumerable<UserAttributeInput> attributes)
        {
            if (attributes is null) return true;
            var keys = attributes.Select(a => a.Key).ToList();
            return keys.Distinct().Count() == keys.Count;
        }
    }
}
