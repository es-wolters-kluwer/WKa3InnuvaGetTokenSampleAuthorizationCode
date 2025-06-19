using Microsoft.AspNetCore.Authentication.Cookies;

namespace WKa3innuvaGetTokenSample.Authentication.Cookies;

public class CookiesBehavior
{
    
    private readonly CookieAuthenticationEvents _events = new();
    
    public void Handler(CookieAuthenticationOptions options)
    {
        options.Cookie.Name = ".Webhook";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.AccessDeniedPath = PathString.FromUriComponent("/Error");
        options.Events = _events;
    }
}