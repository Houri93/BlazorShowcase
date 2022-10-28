using MudBlazor;

namespace BlazorShowcase.Employees;

public interface IEmployeeService
{
    static event Action Changed;
    static void OnChanged() => Changed?.Invoke();

    Task CreateDefaultsAsync();
    Task GenerateNew(int count);
    Task<TableData<Employee>> QueryEmployeesAsync(TableState tableState, string filterText);
}