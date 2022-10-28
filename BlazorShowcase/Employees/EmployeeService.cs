using BlazorShowcase.DataAccess;

using Bogus;

using Microsoft.EntityFrameworkCore;

using MudBlazor;

namespace BlazorShowcase.Employees;

public class EmployeeService : IEmployeeService
{
    private const int DefaultEntriesCount = 10_000;
    private readonly DbCon db;

    public EmployeeService(DbCon db)
    {
        this.db = db;
    }
    public async Task CreateDefaultsAsync()
    {
        if (await db.Employees.AnyAsync())
        {
            return;
        }

        await GenerateNew(DefaultEntriesCount);
    }

    public async Task GenerateNew(int count)
    {
        var faker = new Faker<Employee>();

        faker.RuleFor(a => a.Email, f => f.Person.Email);
        faker.RuleFor(a => a.Birth, f => f.Person.DateOfBirth);
        faker.RuleFor(a => a.Name, f => f.Person.FullName);
        faker.RuleFor(a => a.Address, f => f.Address.FullAddress());
        faker.RuleFor(a => a.PhoneNumber, f => f.Phone.PhoneNumberFormat(1));
        faker.RuleFor(a => a.Created, f => DateTime.Now);

        var employees = faker.Generate(count);

        db.Employees.AddRange(employees);

        await db.SaveChangesAsync();

        IEmployeeService.OnChanged();
    }

    public async Task<TableData<Employee>> QueryEmployeesAsync(TableState tableState, string filterText)
    {
        filterText = filterText?.ToLower();
        var query = db.Employees.AsQueryable();

        if (!string.IsNullOrEmpty(filterText))
        {
            query = query.Where(a =>
            a.Birth.ToString().ToLower().Contains(filterText)
            || a.Email.ToLower().Contains(filterText)
            || a.Name.ToLower().Contains(filterText)
            );
        }

        var filteredCount = query.Count();

        switch (tableState.SortLabel)
        {
            case nameof(Employee.Birth):
                query = query.OrderByDirection(tableState.SortDirection, a => a.Birth);
                break;

            case nameof(Employee.Name):
                query = query.OrderByDirection(tableState.SortDirection, a => a.Name);
                break;

            case nameof(Employee.Email):
                query = query.OrderByDirection(tableState.SortDirection, a => a.Email);
                break;

            case nameof(Employee.Created):
                query = query.OrderByDirection(tableState.SortDirection, a => a.Created);
                break;

            case nameof(Employee.Address):
                query = query.OrderByDirection(tableState.SortDirection, a => a.Address);
                break;

            case nameof(Employee.PhoneNumber):
                query = query.OrderByDirection(tableState.SortDirection, a => a.PhoneNumber);
                break;
        }

        var skipCount = tableState.Page * tableState.PageSize;

        query = query
            .Skip(skipCount)
            .Take(tableState.PageSize);

        var inMemory = await query.ToArrayAsync();

        var tableData = new TableData<Employee>()
        {
            TotalItems = filteredCount,
            Items = inMemory,
        };

        return tableData;
    }
}
