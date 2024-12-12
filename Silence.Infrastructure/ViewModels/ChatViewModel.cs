using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using Silence.Infrastructure.Services;
using Silence.Infrastructure.Utils;
using Silence.Infrastructure.ViewModels;

namespace Silence.Infrastructure.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public const string ChatIdQueryKey = "chatId";

        private HubConnection _hubConnection;

        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly ISecureStorageService _secureStorageService;

        private readonly ApiClientService _apiClientService;

        public ObservableCollection<MessageViewModel> Messages { get; private set; }

        public event EventHandler<string> AlertRequested;

        public ChatViewModel(IAuthenticationService authenticationService,
            ApiClientService apiClientService,
            INavigationService navigationService,
            ISecureStorageService secureStorageService)
        {
            Messages = new ObservableCollection<MessageViewModel>();

            _authenticationService = authenticationService;
            _apiClientService = apiClientService;
            _navigationService = navigationService;
            _secureStorageService = secureStorageService;

            DeleteRoomCommand = new RelayCommand(DeleteRoom);
            EditRoomCommand = new RelayCommand(EditRoom);
            SendMessageCommand = new RelayCommand(async () => await SendMessage());
            ApplyCommand = new RelayCommand(async () => await Apply());
        }

        private RoomViewModel _room;
        public RoomViewModel Room { get { return _room; } private set { _room = value; } }

        private bool _isEditing = false;

        private string _newRoomName = string.Empty;
        public string NewRoomName
        {
            get { return _newRoomName; }
            set
            {
                if (!value.IsNullOrEmpty())
                    _newRoomName = value;
            }
        }

        public string NewMessage { get; set; }

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                }
            }
        }

        public async Task InitializeAsync(int roomId, CancellationToken cancellationToken = default)
        {
            var roomResponse = await _apiClientService.GetRoomAsync(roomId, cancellationToken);
            Update(roomResponse);

            var result = await _apiClientService.GetMessagesAsync(this.Room.Name, cancellationToken);
            var resultName = await _secureStorageService.GetAsync(SecureStorageKey.Username);

            Messages = new ObservableCollection<MessageViewModel>();
            foreach (var item in result)
            {
                item.IsCurrentUser = item.FromUserName == resultName;
                Messages.Add(item);
            }

            OnPropertyChanged(nameof(Messages));

            var AccessToken = await _secureStorageService.GetAsync(SecureStorageKey.AccessToken);

            this._hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:7071/chatHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(AccessToken);
                })
                .Build();

            _hubConnection.On<MessageViewModel>("newMessage", async (message) => await FormatMessage(message));
            await _hubConnection.StartAsync(cancellationToken);
            await _hubConnection.SendAsync("Join", this._room.Name, cancellationToken);
        }

        public ICommand DeleteRoomCommand { get; private set; }
        public ICommand EditRoomCommand { get; private set; }
        public ICommand ApplyCommand { get; private set; }

        public ICommand SendMessageCommand { get; private set; }

        private async Task SendMessage()
        {
            var admin = await _secureStorageService.GetAsync(SecureStorageKey.Username);
            MessageViewModel viewModel = new MessageViewModel()
            {
                Avatar = "false",
                Content = NewMessage,
                FromFullName = admin,
                Timestamp = DateTime.Now,
                FromUserName = admin,
                Room = this.Room.Name,
            };

            var response = await _apiClientService.SendMessage(viewModel);

            if (response is not null)
            {
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    NewMessage = string.Empty;
                    OnPropertyChanged(nameof(NewMessage));
                }
            }
        }

        private void Update(RoomViewModel roomViewModel)
        {
            Room = roomViewModel;
            OnPropertyChanged(nameof(Room));
        }

        private async void DeleteRoom()
        {
            string? thisUsername = await _secureStorageService.GetAsync(SecureStorageKey.Username);
            if (thisUsername != _room.Admin)
            {
                AlertRequested?.Invoke(this, "У вас нет прав на изменение этой комнаты.");
                return;
            }

            var response = await _apiClientService.DeleteRoomAsync(Room.Id);

            if (response != null)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await _navigationService.GoToAsync(Route.Welcome, keepHistory: false);
                }
            }
        }

        private async Task FormatMessage(MessageViewModel message)
        {
            var username = await _secureStorageService.GetAsync(SecureStorageKey.Username);

            message.IsCurrentUser = message.FromUserName.Equals(username);

            Messages.Add(message);
            OnPropertyChanged(nameof(Messages));
        }

        private void EditRoom()
        {
            IsEditing = !IsEditing;
            OnPropertyChanged(nameof(IsEditing));
        }

        private async Task Apply()
        {
            string? thisUsername = await _secureStorageService.GetAsync(SecureStorageKey.Username);
            if (thisUsername != _room.Admin)
            {
                AlertRequested?.Invoke(this, "У вас нет прав на изменение этой комнаты.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(NewRoomName))
            {
                var newRoomModel = new RoomViewModel()
                {
                    Id = this.Room.Id,
                    Admin = this.Room.Admin,
                    Name = NewRoomName
                };

                var response = await _apiClientService.EditRoomAsync(this.Room.Id, newRoomModel);

                if (response.IsSuccessStatusCode)
                {
                    Update(newRoomModel);
                }
            }

            IsEditing = false;
            OnPropertyChanged(nameof(IsEditing));
        }
    }
}
