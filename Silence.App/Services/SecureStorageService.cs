using Silence.Infrastructure.Services;

namespace Silence.App.Services;

public class SecureStorageService : ISecureStorageService
{
    public Task<string> GetAsync(SecureStorageKey key) =>
        SecureStorage.GetAsync(key.ToString());

    public Task RemoveAsync(SecureStorageKey key)
    {
        SecureStorage.Remove(key.ToString());
        return Task.CompletedTask;
    }

    public Task SetAsync(SecureStorageKey key, string value) =>
        SecureStorage.SetAsync(key.ToString(), value);

    public Task RemoveAllAsync()
    {
        SecureStorage.RemoveAll();
        return Task.CompletedTask;
    }
}