using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorShowcase.Employees;

[Table(nameof(Employee))]
[Index(nameof(Created))]
public class Employee
{
    [Key] public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Birth { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string FilterText { get; set; }
}
