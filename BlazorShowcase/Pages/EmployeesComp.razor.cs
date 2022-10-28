using BlazorShowcase.Employees;

using Humanizer;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using System.Diagnostics;

namespace BlazorShowcase.Pages;

public partial class EmployeesComp : IDisposable
{
    private MudTable<Employee> table;
    private string filterText = string.Empty;
    private bool loading = false;
    [Inject] IEmployeeService EmployeeService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    protected override void OnInitialized()
    {
        IEmployeeService.Changed += IEmployeeService_Changed;
    }

    private void IEmployeeService_Changed()
    {
        InvokeAsync(async () =>
        {
            await table.ReloadServerData();
            Snackbar.Add("New employee added", Severity.Success);
        });
    }

    private async Task<TableData<Employee>> QueryEmployeesAsync(TableState tableState)
    {
        var tableData = await EmployeeService.QueryEmployeesAsync(tableState, filterText);
        return tableData;
    }

    private async Task FilterTextChanged(string text)
    {
        filterText = text;
        await table.ReloadServerData();
    }

    private static string MakeDobString(DateOnly dob)
    {
        var age = DateTime.Now.Humanize(utcDate: false, dateToCompareAgainst: dob.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.Zero))).Replace(" from now", "");
        return $"{dob.ToString()} ({age})";
    }
    private static string MakeCreatedString(DateTime created)
    {
        return $"{created.ToString()} ({created.Humanize(utcDate: false)})";
    }

    public void Dispose()
    {
        IEmployeeService.Changed -= IEmployeeService_Changed;
    }
}
