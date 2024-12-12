using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Silence.Infrastructure.ViewModels;
using Silence.Infrastructure.DataContracts;
using Silence.Infrastructure.ViewModels;

namespace Silence.Infrastructure.Services;

public class ApiClientService
{
    public const string AutorizedHttpClient = "AutorizedHttpClient";

    private readonly ILogger<ApiClientService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _serverAddress;

    public ApiClientService(ILogger<ApiClientService> logger,
        IHttpClientFactory httpClientFactory,
        string serverAddress)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _serverAddress = $"{serverAddress}/api";

    }

    #region Auth

    public async Task<LoginResponse> LoginAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient()
            .PostAsJsonAsync($"{_serverAddress}/auth/login", new LoginRequest
                {
                    Username = username,
                    Password = password
                },
                cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content
                .ReadFromJsonAsync<LoginResponse>(
                    cancellationToken: cancellationToken);

            if (responseContent == null)
            {
                throw new JsonException("Response has unknown format");
            }

            return responseContent;
        }
        else
        {
            throw new HttpRequestException(
                $"Error logging in: {response.ReasonPhrase}", null,
                response.StatusCode);
        }
    }

    public async Task<bool> RegisterAsync(string username, string password, string Email, string FullName,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient()
            .PostAsJsonAsync($"{_serverAddress}/auth/register", new RegisterRequest
                {
                    UserName = username,
                    Password = password,
                    Email = Email,
                    FullName = FullName
                },
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Error logging in: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        return true;
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken,
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient()
            .PostAsJsonAsync($"{_serverAddress}/auth/refresh-token", new RefreshTokenRequest
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                },
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Refresh token fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        var responseContent = await response.Content
            .ReadFromJsonAsync<RefreshTokenResponse>(
                cancellationToken: cancellationToken);

        if (responseContent == null)
        {
            throw new JsonException("Response has unknown format");
        }

        return responseContent;
    }

    public async Task<bool> ValidateTokenAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .GetAsync($"{_serverAddress}/auth/validate-token", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Validate token fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        return true;
    }

    public async Task<bool> LogoutAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .GetAsync($"{_serverAddress}/auth/logout", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Logout fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        return true;
    }

    #endregion

    #region New

    public async Task<IEnumerable<RoomViewModel>> GetRoomsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .GetAsync($"{_serverAddress}/Rooms", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Get teams request fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        var responseContent = await response.Content
            .ReadFromJsonAsync<IEnumerable<RoomViewModel>>(
                cancellationToken: cancellationToken);

        if (responseContent == null)
        {
            throw new JsonException("Response has unknown format");
        }

        return responseContent;
    }

    public async Task<IEnumerable<MessageViewModel>> GetMessagesAsync(
        string roomName,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .GetAsync($"{_serverAddress}/Messages/Room/{roomName}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Get teams request fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        var responseContent = await response.Content
            .ReadFromJsonAsync<IEnumerable<MessageViewModel>>(
                cancellationToken: cancellationToken);

        if (responseContent == null)
        {
            throw new JsonException("Response has unknown format");
        }

        return responseContent;
    }

    public async Task<HttpResponseMessage> SendMessage(MessageViewModel messageViewModel,
         CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .PostAsJsonAsync($"{_serverAddress}/Messages", messageViewModel, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Get teams request fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        return response;
    }

    public async Task<HttpResponseMessage> CreateRoomAsync(string? admin,
        CancellationToken cancellationToken = default
        )
    {
        admin = admin is not null ? admin : "123123";

        string json = JsonSerializer.Serialize(new RoomViewModel() { Id = 0, Name = "New Room", Admin = admin }).ToLower();

        

        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .PostAsJsonAsync($"{_serverAddress}/Rooms",
                new RoomViewModel() { Id = 0, Name = $"NewRoom_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}_{DateTime.UtcNow}".Replace(' ', '_'), Admin = admin }, 
                    cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Get team request fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        return response;
    }

    public async Task<RoomViewModel> GetRoomAsync(int roomId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .GetAsync($"{_serverAddress}/Rooms/{roomId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Get team request fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        return await ParseGetRoomResponseAsync(response, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteRoomAsync(int roomId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .DeleteAsync($"{_serverAddress}/Rooms/{roomId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Get team request fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        return response;
    }

    public async Task<HttpResponseMessage> EditRoomAsync(int roomID, RoomViewModel room,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClientFactory
            .CreateClient(AutorizedHttpClient)
            .PutAsJsonAsync($"{_serverAddress}/Rooms/{roomID}", room, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Get team request fails: {response.ReasonPhrase}", null,
                response.StatusCode);
        }

        return response;
    }

    private async Task<RoomViewModel> ParseGetRoomResponseAsync(
        HttpResponseMessage? response,
        CancellationToken cancellationToken = default)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        var responseContent = await response.Content
            .ReadFromJsonAsync<RoomViewModel>(
                cancellationToken: cancellationToken);

        if (responseContent == null)
        {
            throw new JsonException("Response has unknown format");
        }

        return responseContent;
    }

    #endregion
}

