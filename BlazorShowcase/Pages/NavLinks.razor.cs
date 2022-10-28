using BlazorShowcase.Accounts;
using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace BlazorShowcase.Pages;

public sealed partial class NavLinks
{
    [Inject] IAuthService Auth { get; set; }  
    [Inject] NavigationManager Nav { get; set; }

  
    protected override void OnInitialized()
    {
        Nav.LocationChanged += (_, _) => InvokeAsync(StateHasChanged);
    }
   
    private string ActiveLinkStyle(string href)
    {
        return ActiveLink(href) ? "text-decoration:solid underline" : "";
    }
  
    
    private bool ActiveLink(string href)
    {
        var currentPage = Nav.ToBaseRelativePath(Nav.Uri);
        if (href == string.Empty)
        {
            return currentPage == href;
        }
        return currentPage.ToLower().StartsWith(href.ToLower());
    }
}
