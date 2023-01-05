using MudBlazor;

namespace BlazorShowcase.Employees;

public interface IEmployeeService
{
    static event Action<string> NotifyNewName;
    static event Action Changed;
    static void OnChanged() => Changed?.Invoke();
    static void OnNotifyNewName(string name) => NotifyNewName?.Invoke(name);
    Task AddAsync(Employee model);
    Task<int> CountAsync();
    Task<Employee> GetByIdAsync(Guid id);
    Task CreateDefaultsAsync();
    Task GenerateNewAsync(int count);
    Task<Employee[]> GetManyByIdAsync(params Guid[] ids);
    Task<TableData<Employee>> QueryEmployeesAsync(TableState tableState, string filterText);
}