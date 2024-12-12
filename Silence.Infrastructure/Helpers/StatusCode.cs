namespace Silence.Infrastructure.Helpers
{
    public enum StatusCode
    {
        FileAlreadyExists = 409,
        FileNotFound = 404,
        UserNotFound = 0,
        UserAlreadyExists = 409,
        OK = 200,
        InternalServerError = 500
    }
}
