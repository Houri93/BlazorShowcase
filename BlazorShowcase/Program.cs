
using Blazored.LocalStorage;

using BlazorShowcase.Accounts;
using BlazorShowcase.DataAccess;
using BlazorShowcase.Employees;
using BlazorShowcase.Streamers;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Services;

using (var db = new DbCon())
{
    await db.Database.EnsureCreatedAsync();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.AddAuthentication();
services.AddAuthorization();

services.AddRazorPages();
services.AddServerSideBlazor();//.AddCircuitOptions(a => a.DetailedErrors = true);
services.AddMudServices();

services.AddHttpContextAccessor();
services.AddBlazoredLocalStorage();
services.AddDbContext<DbCon>();
services.AddScoped<IEmployeeService, EmployeeService>();
services.AddScoped<AuthenticationStateProvider, AuthService>();
services.AddScoped<IAuthService>(sp => sp.GetService<AuthenticationStateProvider>() as IAuthService);

services.AddSingleton<IWaveGeneratorService, WaveGeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await Init();
BeginGenerating();



app.Run();

async Task Init()
{
    using var scope = app.Services.CreateScope();
    var sp = scope.ServiceProvider;
    var employeeService = sp.GetService<IEmployeeService>();
    await employeeService.CreateDefaultsAsync();

    var authService = sp.GetService<IAuthService>();
    await authService.CreateDefaultsAsync();

    var waveGen = sp.GetService<IWaveGeneratorService>();
    waveGen.BeginGeneratingTask();
}

void BeginGenerating()
{
    Task.Run(async () =>
    {
        while (true)
        {
            var scope = app.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var employeeService = sp.GetService<IEmployeeService>();
            await employeeService.GenerateNew(1);
            await Task.Delay(5000);
        }
    });
}