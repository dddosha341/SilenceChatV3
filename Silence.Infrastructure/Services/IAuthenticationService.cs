
namespace Silence.Infrastructure.Services;

public interface IAuthenticationService
{
    event EventHandler<bool> OnAuthenticationStateChanged;

    /// <summary>
    /// Current authentication state. Doesn't mean the check on server side.
    /// </summary>
    public bool IsAuthenticated { get; }

    public int? UserId { get; }

    Task<bool> AuthenticateAsync(string login, string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the user is authenticated on server
    /// </summary>
    Task<bool> CheckIsAuthenticatedAsync(CancellationToken cancellationToken = default);

    Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default);

    Task<bool> LogoutAsync(CancellationToken cancellationToken = default);
}

