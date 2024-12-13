using System.Windows.Input;
using Silence.Infrastructure.Services;
using Silence.Infrastructure.Utils;
using Silence.Infrastructure.ViewModels;

namespace Silence.App.ViewModels;

public class AppShellViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authenticationService;

    public AppShellViewModel(
        IAuthenticationService authenticationService,
        INavigationService navigationService)
    {
        _authenticationService = authenticationService;
        _navigationService = navigationService;

        LogoutCommand = new RelayCommand(HandleLogoutAsync);
        _authenticationService.OnAuthenticationStateChanged += HandleAuthenticationStateChanged;
    }

    private void HandleAuthenticationStateChanged(object? sender, bool e)
    {
        base.OnPropertyChanged(nameof(IsAuthenticated));
    }

    public bool IsAuthenticated => _authenticationService.IsAuthenticated;

    public ICommand LogoutCommand { get; }

    private async Task HandleLogoutAsync()
    {
        await _authenticationService.LogoutAsync();
        await _navigationService.GoToAsync(Route.Login, keepHistory: false);
    }
}