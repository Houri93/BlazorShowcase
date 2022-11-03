using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using BlazorShowcase.Accounts;

namespace BlazorShowcase.Pages;

public partial class LoginComp
{
    private const string EnterKeyName = "Enter";
    private const string IncorrectCredentialsError = "Incorrect credentials";

    [Inject] IAuthService Auth { get; set; }
    [Inject] NavigationManager Nav { get; set; }

    private string username, password, error;
    private MudTextField<string> passwordInput;
    private bool showPassword;

    private void TogglePasswordShow() => showPassword = !showPassword;
    private InputType PasswordInputType() => showPassword ? InputType.Text : InputType.Password;
    private string PasswordInputIcon() => showPassword ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;

    private async Task TryLoginAsync()
    {
        ClearError();

        if (await Auth.TryLoginAsync(username, Hasher.Hash(password)))
        {
            Nav.NavigateTo("/");
        }
        else
        {
            error = IncorrectCredentialsError;
        }
    }

    private async Task UsernameEnterClicked(KeyboardEventArgs args)
    {
        if (args.Key != EnterKeyName)
        {
            return;
        }

        await passwordInput.FocusAsync();
        ClearError();
    }

    private async Task PasswordEnterClicked(KeyboardEventArgs args)
    {
        if (args.Key != EnterKeyName)
        {
            return;
        }

        await TryLoginAsync();
    }

    private void ClearError()
    {
        error = null;
    }

    private bool HasError() => error is not null;
}
