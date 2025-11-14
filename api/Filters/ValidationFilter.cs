using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;

namespace UserNotepad.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null) continue;

                await Validate(argument, _serviceProvider);
            }

            await next();
        }

        private async Task Validate(object model, IServiceProvider provider)
        {
            if (model is null)
                return;

            var validatorType = typeof(IValidator<>).MakeGenericType(model.GetType());
            var validator = provider.GetService(validatorType) as IValidator;

            if (validator is not null)
            {
                var context = new ValidationContext<object>(model);
                var result = await validator.ValidateAsync(context);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);
            }

            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                if (value is null)
                    continue;

                if (value is IEnumerable collection && value is not string)
                {
                    foreach (var item in collection)
                    {
                        if (item is not null)
                            await Validate(item, provider);
                    }

                    continue;
                }

                if (!property.PropertyType.IsPrimitive &&
                    property.PropertyType != typeof(string) &&
                    !property.PropertyType.IsEnum &&
                    !property.PropertyType.IsValueType)
                {
                    await Validate(value, provider);
                }
            }
        }
    }
}
