using FluentValidation;
using System.Globalization;
using UserNotepad.Entities;

namespace UserNotepad.Models.Validators
{
    public class UserAttributeInputValidator : AbstractValidator<UserAttributeInput>
    {
        public UserAttributeInputValidator()
        {
            RuleFor(x => x.Key).NotEmpty().WithMessage("Key is required!");
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required!");
            RuleFor(x => x.ValueType).NotNull().WithMessage("Value type is required!");
            When(x => x.ValueType == AttributeTypeEnum.@int, () =>
            {
                RuleFor(x => x.Value)
                    .Must(v => int.TryParse(v, out _))
                    .WithMessage("Value must be a valid integer!");
            });

            When(x => x.ValueType == AttributeTypeEnum.@double, () =>
            {
                RuleFor(x => x.Value)
                    .Must(v => double.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    .WithMessage("Value must be a valid double!");
            });

            When(x => x.ValueType == AttributeTypeEnum.@bool, () =>
            {
                RuleFor(x => x.Value)
                    .Must(v => v == "true" || v == "false")
                    .WithMessage("Value must be 'true' or 'false'!");
            });

            When(x => x.ValueType == AttributeTypeEnum.Date, () =>
            {
                RuleFor(x => x.Value)
                    .Must(v =>
                        DateTime.TryParseExact(v, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out _))
                    .WithMessage("Value must be a valid date (yyyy-MM-dd)!");
            });
        }
    }
}