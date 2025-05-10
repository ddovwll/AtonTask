namespace AtonTask.WebApi.Utils;

public class CookieOptionsProvider
{
    public CookieOptions GetRefreshOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(30),
            SameSite = SameSiteMode.Strict,
            Secure = false,
            Path = "/api/user/refresh",
            Domain = "localhost"
        };
    }
}