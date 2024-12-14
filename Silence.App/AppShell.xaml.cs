using Silence.App.ViewModels;
using Silence.Infrastructure.Services;
using Silence.App.Pages;
using Silence.App.Services;

namespace Silence.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        RegisterRoutes();

        BindingContext = MauiProgram.Services.GetRequiredService<AppShellViewModel>();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute(Route.Login.MapRouteToPath(), typeof(LoginPage));
    }
}