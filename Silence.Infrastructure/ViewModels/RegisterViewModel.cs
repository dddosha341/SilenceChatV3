using System.Windows.Input;
using Silence.Infrastructure.Utils;
using Silence.Infrastructure.Services;

namespace Silence.Infrastructure.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly ApiClientService _apiClientService;

    private string _error;

    public RegisterViewModel(
        ApiClientService apiClientService,
        INavigationService navigationService)
    {
        _navigationService = navigationService;
        _apiClientService = apiClientService;

        RegisterCommand = new RelayCommand(HandleRegister);
    }

    public string Login { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }

    public string Error
    {
        get => _error;
        set => SetField(ref _error, value);
    }

    public ICommand RegisterCommand { get; }

    public ICommand LoginCommand { get; }


    private async Task HandleRegister()
    {
        try
        {
            
            var isRegistered = await _apiClientService
                .RegisterAsync(Login, Password, Email, FullName);

            if (isRegistered)
            {
                Error = string.Empty;
                await _navigationService.GoToAsync(Route.Login, keepHistory: false);
            }
        }
        catch (Exception e)
        {
            Error = $"Please use another username to register.\n{e.Message}";
        }
    }
}

