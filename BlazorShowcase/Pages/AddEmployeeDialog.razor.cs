using BlazorShowcase.Employees;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace BlazorShowcase.Pages;

public partial class AddEmployeeDialog
{
    private readonly EmployeeValidator validator = new();
    private readonly Employee model = new();
    private MudForm form;

    [CascadingParameter] MudDialogInstance Dialog { get; set; }
    [Inject] IEmployeeService EmployeeService { get; set; }

    private async Task TryAdd()
    {
        await form.Validate();
        if (!form.IsValid)
        {
            return;
        }

        await EmployeeService.AddAsync(model);
        Dialog.Close();
    }
}
