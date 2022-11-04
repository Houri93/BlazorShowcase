using BlazorShowcase.Employees;

using Humanizer;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using MudBlazor;

using System.Diagnostics;

using static MudBlazor.CategoryTypes;

namespace BlazorShowcase.Pages;
public partial class EmployeesComp : IDisposable
{
    private MudTable<Employee> table;
    private int totalCount;

    private Guid[] employeesIds = Array.Empty<Guid>();
    private string filterText = string.Empty;
    [Inject] IEmployeeService EmployeeService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] IDialogService DialogService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IEmployeeService.Changed += IEmployeeService_Changed;
        IEmployeeService.NotifyNewName += IEmployeeService_NotifyNewName;
    }

    private void IEmployeeService_NotifyNewName(string name)
    {
        InvokeAsync(() =>
        {
            Snackbar.Add($"{name}, added as new employee.", Severity.Success);
        });
    }
    private async ValueTask<ItemsProviderResult<Employee>> ItemsProvider(ItemsProviderRequest request)
    {
        var ids = employeesIds.Skip(request.StartIndex).Take(request.Count).ToArray();
        var items = await EmployeeService.GetManyByIdAsync(ids);
        var result = new ItemsProviderResult<Employee>(items, employeesIds.Length);
        return result;
    }

    public void GetItems()
    {

    }

    private void IEmployeeService_Changed()
    {
        InvokeAsync(table.ReloadServerData);
    }

    private async Task<TableData<Employee>> QueryEmployeesAsync(TableState tableState)
    {
        var queryResult = await EmployeeService.QueryEmployeesAsync(tableState, filterText);
        employeesIds = queryResult.Ids;
        totalCount = queryResult.totalCount;

        StateHasChanged();

        return new() { Items = Enumerable.Range(0, 10).Select(a => new Employee()), TotalItems = totalCount };
    }

    private async Task FilterTextChanged(string text)
    {
        filterText = text;
        await table.ReloadServerData();
    }

    private void ShowAddDialog()
    {
        DialogService.Show<AddEmployeeDialog>("New Employee", new DialogOptions()
        {
            CloseButton = true,
            CloseOnEscapeKey = true,
            DisableBackdropClick = false,
            FullScreen = false,
            FullWidth = false,
            MaxWidth = MaxWidth.ExtraLarge,
            NoHeader = false,
            Position = DialogPosition.Center,
        });
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
        IEmployeeService.NotifyNewName -= IEmployeeService_NotifyNewName;
    }
}
