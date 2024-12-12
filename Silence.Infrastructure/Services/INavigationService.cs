
namespace Silence.Infrastructure.Services;

public interface INavigationService
{
    /// <summary>
    /// Navigates to the specific page by <paramref name="route"/>
    /// </summary>
    /// <param name="route">Determines the page</param>
    /// <param name="parameters">
    /// Should include parameters if needed.
    /// Each parameter should have unique key and value
    /// </param>
    /// <param name="keepHistory">Means keep naviagtion stack or not</param>
    Task GoToAsync(Route route, IDictionary<string, object>? parameters = null,
        bool keepHistory = true);

    Task GoBackAsync();
}

public enum Route
{
    Root,
    Back,
    Login,
    Register,
    Welcome,
    ChatRoom,
    EditProfile,
}

