namespace Silence.Infrastructure.DataContracts;

public class LoginResponse
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; }
}

