using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using WKa3innuvaGetTokenSample.Authentication.Cookies;
using WKa3innuvaGetTokenSample.Authentication.WoltersKluwerIdentity;

namespace WKa3innuvaGetTokenSample.Extensions;

internal static class AuthenticationExtensions
{

    public static IServiceCollection AddWoltersKluwerIdentity(this IServiceCollection services,
        IConfiguration configuration)
    {

        var woltersKluwerIdentityOptions = services.AddWoltersKluwerIdentityOptions(configuration);
        var cookiesHandler = new CookiesBehavior();
        var openIdConnectHandler = new WoltersKluwerOpenIdConnectBehavior(woltersKluwerIdentityOptions);

        services
            .AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookiesHandler.Handler)
            .AddOpenIdConnect(openIdConnectHandler.Handler);
        
        return services;
    }
    
    private static WoltersKluwerIdentityOptions AddWoltersKluwerIdentityOptions(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddOptions<WoltersKluwerIdentityOptions>()
            .BindConfiguration(WoltersKluwerIdentityOptions.SectionName)
            .ValidateOnStart();
        
        using var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IOptions<WoltersKluwerIdentityOptions>>().Value;
        
    }

}