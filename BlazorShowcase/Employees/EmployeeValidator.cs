using FluentValidation;

namespace BlazorShowcase.Employees;

public class EmployeeValidator : Validator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(a => a.Name).NotEmpty();
        RuleFor(a => a.Email).EmailAddress();
        RuleFor(a => a.Birth).NotEmpty().LessThanOrEqualTo(DateTime.Now);
        RuleFor(a => a.Address).NotEmpty();
        RuleFor(a => a.PhoneNumber).NotEmpty();
    }
}
