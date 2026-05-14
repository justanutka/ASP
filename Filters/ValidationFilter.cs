using System.ComponentModel.DataAnnotations;

namespace UniDesc.Web.Filters
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var argument = context.Arguments.OfType<T>().FirstOrDefault();

            if (argument == null)
            {
                return await next(context);
            }

            var validationContext = new ValidationContext(argument);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(
                argument,
                validationContext,
                validationResults,
                validateAllProperties: true);

            if (!isValid)
            {
                var errors = validationResults
                    .GroupBy(result => result.MemberNames.FirstOrDefault() ?? string.Empty)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .Select(result => result.ErrorMessage ?? "Validation error")
                            .ToArray());

                Console.WriteLine("[ValidationFilter] Request rejected before service.");

                return Results.ValidationProblem(errors);
            }

            return await next(context);
        }
    }
}