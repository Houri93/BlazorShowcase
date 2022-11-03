using BlazorShowcase.DataAccess;

using Bogus;

using Microsoft.EntityFrameworkCore;

using MudBlazor;

namespace BlazorShowcase.Employees;

public class EmployeeService : IEmployeeService
{
    private const int DefaultEntriesCount = 100_000;
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

        await GenerateNewAsync(DefaultEntriesCount);
    }

    public async Task GenerateNewAsync(int count)
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

        if (count == 1)
        {
            IEmployeeService.OnNotifyNewName(employees[0].Name);
        }
    }

    public async Task AddAsync(Employee model)
    {
        model.Id = Guid.NewGuid();
        model.Created = DateTime.Now;

        db.Employees.Add(model);
        await db.SaveChangesAsync();

        IEmployeeService.OnChanged();
        IEmployeeService.OnNotifyNewName(model.Name);
    }

    public async Task<int> CountAsync()
    {
        return await db.Employees.CountAsync();
    }

    public Task<Employee> GetByIdAsync(Guid id) => db.Employees.FindAsync(id).AsTask();
    public async Task<Employee[]> GetManyByIdAsync(Guid[] ids)
    {
        var randomlyOrderedEmployees = await db.Employees
            .Where(a => ids.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id);

        var employees = new List<Employee>();

        foreach (var id in ids)
        {
            employees.Add(randomlyOrderedEmployees[id]);
        }

        return employees.ToArray();
    }

    public async Task<(TableData<Employee> tableData, int totalCount)> QueryEmployeesAsync(TableState tableState, string filterText)
    {
        filterText = filterText?.ToLower();
        var query = db.Employees.AsQueryable();

        if (!string.IsNullOrEmpty(filterText))
        {
            query = query.Where(a =>
            a.Birth.Value.ToString().ToLower().Contains(filterText)
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

        //var skipCount = tableState.Page * tableState.PageSize;

        //query = query
        //    .Skip(skipCount)
        //    .Take(tableState.PageSize);

        var inMemory = await query.Select(a => new Employee { Id = a.Id }).ToArrayAsync();

        var tableData = new TableData<Employee>()
        {
            TotalItems = filteredCount,
            Items = inMemory,
        };

        return (tableData, await db.Employees.CountAsync());
    }
}
