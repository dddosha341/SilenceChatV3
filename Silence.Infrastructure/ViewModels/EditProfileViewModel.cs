using Silence.Infrastructure.Services;

namespace Silence.Infrastructure.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        private readonly ISecureStorageService _secureStorageService;

        private readonly IAuthenticationService _authenticationService;

        private string? _name;

        

        public EditProfileViewModel(INavigationService navigationService,
           ISecureStorageService secureStorageService,
           IAuthenticationService authenticationService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            _secureStorageService = secureStorageService;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _name = await _secureStorageService.GetAsync(SecureStorageKey.Username);

            OnPropertyChanged(nameof(Name));
        }

        public string? Name
        {
            get { return _name; }
        }

    }
}
