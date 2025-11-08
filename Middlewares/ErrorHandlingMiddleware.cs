using FluentValidation;

namespace UserNotepad.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);

            } catch (ValidationException ex) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errors = ex.Errors.Select(x => new
                {
                    x.PropertyName,
                    x.ErrorMessage,
                });

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Wrong input",
                    errors
                });

            } catch (Exception ex) {
                _logger.LogError(ex, "Unhandled exception!");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Internal server error occured");
            }
        }
    }
}
