using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using Silence.Infrastructure.Utils;
using Silence.Infrastructure.DataContracts;
using Silence.Infrastructure.Services;
using System.Threading;

namespace Silence.Infrastructure.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        private readonly INavigationService _navigationService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly ApiClientService _apiClientService;

        public IEnumerable<RoomViewModel> Rooms { get; private set; }

        public ICommand CreateRoomCommand { get; }

        public WelcomeViewModel(INavigationService navigationService, 
            ApiClientService apiClientService,
            ISecureStorageService secureStorageService)
        {
            _httpClient = new HttpClient();

            _navigationService = navigationService;
            _apiClientService = apiClientService;
            _secureStorageService = secureStorageService;

            CreateRoomCommand = new RelayCommand(CreateRoomButton);
        }

        private RoomViewModel _selectedRoom;

        public RoomViewModel? SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                if (value is not null && SetField(ref _selectedRoom, value))
                {
                    HandleSelectedItemChanged();
                }
            }
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            Rooms = await _apiClientService.GetRoomsAsync(cancellationToken);
            OnPropertyChanged(nameof(Rooms));
        }

        private async Task CreateRoomButton()
        {
            await CreateRoom();
        }


        private async Task CreateRoom(CancellationToken cancellationToken = default)
        {
            var admin = await _secureStorageService.GetAsync(SecureStorageKey.Username);

            await _apiClientService.CreateRoomAsync(admin);

            Rooms = await _apiClientService.GetRoomsAsync(cancellationToken);
            OnPropertyChanged(nameof(Rooms));
        }

        private async void HandleSelectedItemChanged()
        {
            try
            {
                await _navigationService.GoToAsync(Route.ChatRoom,
                    new Dictionary<string, object>()
                    {
                    { ChatViewModel.ChatIdQueryKey, _selectedRoom.Id }
                    });
            }
            catch
            {
                // TODO: Handle exception here
            }
        }
    }

    
}
