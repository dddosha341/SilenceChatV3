
namespace Silence.Infrastructure.Services;

public interface ISecureStorageService
{
    Task SetAsync(SecureStorageKey key, string value);

    Task<string> GetAsync(SecureStorageKey key);

    Task RemoveAsync(SecureStorageKey key);

    Task RemoveAllAsync();
}

public enum SecureStorageKey
{
    AccessToken,
    RefreshToken,
    UserId,
    Username,
    FullName,
}

