using Microsoft.Extensions.Logging;
using Silence.Infrastructure.Services;

namespace Silence.App;

public partial class App : Application
{
    private readonly ILogger<App> _logger;
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authenticationService;

    public App(ILogger<App> logger,
        INavigationService navigationService,
        IAuthenticationService authenticationService)
    {
        InitializeComponent();

        _logger = logger;
        _navigationService = navigationService;
        _authenticationService = authenticationService;

        MainPage = new AppShell();
    }

    protected async override void OnResume()
    {
        base.OnResume();

        try
        {
            await SetStartupPage();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to check the authentication status");
        }
    }

    protected override async void OnStart()
    {
        base.OnStart();

        try
        {
            await SetStartupPage();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to check the authentication status");
        }
    }

    private async Task SetStartupPage()
    {
        var isAuthenticated = await _authenticationService
                .CheckIsAuthenticatedAsync();

        var startupRoute = isAuthenticated
            ? Route.Welcome
            : Route.Login;

        await _navigationService.GoToAsync(startupRoute, keepHistory: false);
    }
}