using Blazored.LocalStorage;

using BlazorShowcase.Accounts;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using MudBlazor;

namespace BlazorShowcase.Pages;

public partial class MainLayout
{
    public const string HideOnMobileClass = "d-none d-xs-none d-sm-none d-md-flex d-lg-flex d-xl-flex d-xx-flex";
    public const string ShowOnMobileClass = "d-flex d-xs-flex d-sm-flex d-md-none d-lg-none d-xl-none d-xx-none";
    public const string NavButtonStyle = "text-decoration:underline solid 1px";

    private bool darkMode;
    [Inject] ILocalStorageService LocalStorageService { get; set; }
    [Inject] IAuthService AuthService { get; set; }
    [Inject] IDialogService DialogService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    protected override async Task OnInitializedAsync()
    {
        darkMode = await LocalStorageService.GetItemAsync<bool>(nameof(darkMode));
    }

    private async Task DarkModeChangedAsync(bool toggled)
    {
        darkMode = toggled;
        await LocalStorageService.SetItemAsync<bool>(nameof(darkMode), darkMode);
    }

    private async Task LogoutAsync()
    {
        await AuthService.LogoutAsync();
        Snackbar.Clear();
    }
    private void ShowMenuDialog()
    {
        DialogService.Show<NavMenuDialog>("Navigation Menu");
    }
}
