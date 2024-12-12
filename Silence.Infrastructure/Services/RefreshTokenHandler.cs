using System.Net;
using System.Net.Http.Headers;

namespace Silence.Infrastructure.Services;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly ISecureStorageService _secureStorageService;
    private readonly IAuthenticationService _authenticationService;

    public RefreshTokenHandler(ISecureStorageService secureStorageService, IAuthenticationService authenticationService)
    {
        _secureStorageService = secureStorageService;
        _authenticationService = authenticationService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // получаем access токен и добавляем к заголовкам запроса
        var accessToken = await _secureStorageService.GetAsync(SecureStorageKey.AccessToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            var isRefreshed = await _authenticationService.RefreshTokenAsync(cancellationToken);

            if (isRefreshed)
            {
                // Получаем новый access token и повторяем запрос
                accessToken = await _secureStorageService.GetAsync(SecureStorageKey.AccessToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}

