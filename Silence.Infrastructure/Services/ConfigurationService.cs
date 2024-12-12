using Microsoft.Extensions.Configuration;

namespace Silence.Server.Services;



public class ConfigurationService
{
    private IConfiguration _configuration;

    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string JwtKey => _configuration["Jwt:Key"];

    public double JwtAccessTokenExpirationMinutes =>
        double.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]);

    public double JwtRefreshTokenExpirationDays =>
        double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]);

    public string JwtIssuer => _configuration["Jwt:Issuer"];

    public string JwtAudience => _configuration["Jwt:Audience"];
}

