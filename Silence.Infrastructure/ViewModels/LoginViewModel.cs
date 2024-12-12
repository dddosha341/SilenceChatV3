using System.Windows.Input;
using Silence.Infrastructure.Utils;
using Silence.Infrastructure.Services;

namespace Silence.Infrastructure.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authenticationService;

    private string _login;
    private string _password;

    private string _error;

    public LoginViewModel(
        IAuthenticationService authenticationService,
        INavigationService navigationService)
    {
        _authenticationService = authenticationService;
        _navigationService = navigationService;

        LoginCommand = new RelayCommand(HandleLoginAsync);
        RegisterCommand = new RelayCommand(HandleRegister);
    }

    public string Login
    {
        get => _login;
        set => SetField(ref _login, value);
    }

    public string Password
    {
        get => _password;
        set => SetField(ref _password, value);
    }

    public string Error
    {
        get => _error;
        set => SetField(ref _error, value);
    }

    public ICommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }

    private async Task HandleLoginAsync()
    {
        bool isAuthenticated = false;

        try
        {
            isAuthenticated = await _authenticationService
                .AuthenticateAsync(login: Login, password: Password);

            if (isAuthenticated)
            {
                Error = string.Empty;
                Login = string.Empty;
                Password = string.Empty;
                await _navigationService.GoToAsync(Route.Welcome, keepHistory: false);
            }
        }
        finally
        {
            if (!isAuthenticated)
            {
                Error = "Authentication failed.\n" +
                    "Please check your username and password and try again.";
            }
        }
    }

    private Task HandleRegister()
    {

        Error = string.Empty;
        return _navigationService.GoToAsync(Route.Register);
    }
}

