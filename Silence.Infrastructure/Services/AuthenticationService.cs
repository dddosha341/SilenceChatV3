using Microsoft.Extensions.Logging;

namespace Silence.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    public event EventHandler<bool> OnAuthenticationStateChanged;

    private readonly ILogger<AuthenticationService> _logger;
    private readonly ISecureStorageService _secureStorageService;
    private readonly ApiClientService _apiClientService;

    private bool _isAuthenticated;

    public int? UserId { get; private set; }

    public AuthenticationService(ILogger<AuthenticationService> logger,
        ISecureStorageService secureStorageService,
        ApiClientService apiClientService)
    {
        _logger = logger;
        _secureStorageService = secureStorageService;
        _apiClientService = apiClientService;
    }

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        set
        {
            if (_isAuthenticated != value)
            {
                _isAuthenticated = value;
                OnAuthenticationStateChanged?.Invoke(this, _isAuthenticated);
            }
        }
    }

    public async Task<bool> AuthenticateAsync(string login, string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiClientService
                .LoginAsync(
                    username: login,
                    password: password,
                    cancellationToken);

            await _secureStorageService.SetAsync(SecureStorageKey.AccessToken, response.AccessToken);
            await _secureStorageService.SetAsync(SecureStorageKey.RefreshToken, response.RefreshToken);
            await _secureStorageService.SetAsync(SecureStorageKey.UserId, response.UserId.ToString());
            await _secureStorageService.SetAsync(SecureStorageKey.Username, response.Username);

            UserId = response.UserId;
            IsAuthenticated = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to authenticate");
            UserId = null;
            IsAuthenticated = false;
        }

        return IsAuthenticated;
    }

    public async Task<bool> CheckIsAuthenticatedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _apiClientService.ValidateTokenAsync(cancellationToken);
            var rawUserId = await _secureStorageService.GetAsync(SecureStorageKey.UserId);

            if (int.TryParse(rawUserId, out int userId))
            {
                UserId = userId;
                IsAuthenticated = true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to check for authentication status");
            UserId = null;
            IsAuthenticated = false;
        }

        return IsAuthenticated;
    }

    public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await _secureStorageService.GetAsync(SecureStorageKey.AccessToken);
            var refreshToken = await _secureStorageService.GetAsync(SecureStorageKey.RefreshToken);

            var response = await _apiClientService
                .RefreshTokenAsync(
                    accessToken: accessToken,
                    refreshToken: refreshToken,
                    cancellationToken);

            await _secureStorageService.SetAsync(SecureStorageKey.AccessToken, response.AccessToken);
            await _secureStorageService.SetAsync(SecureStorageKey.RefreshToken, response.RefreshToken);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to refresh token");
            return false;
        }
    }

    public async Task<bool> LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _apiClientService.LogoutAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to logout");
        }
        finally
        {
            await _secureStorageService.RemoveAllAsync();
            UserId = null;
            IsAuthenticated = false;
        }

        return true;
    }
}

