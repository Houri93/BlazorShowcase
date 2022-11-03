using FluentValidation;

namespace BlazorShowcase.Employees;

public class Validator<T> : AbstractValidator<T>
{
    public Validator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
    }   

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue() => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model, x => x.IncludeProperties(propertyName)));

        if (result.IsValid)
        {
            return Enumerable.Empty<string>();
        }

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
