using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace WKa3innuvaGetTokenSample.Authentication.WoltersKluwerIdentity;

public class WoltersKluwerOpenIdConnectBehavior(WoltersKluwerIdentityOptions woltersKluwerIdentityOptions)
{

    private readonly OpenIdConnectEvents _events = new()
    {
        OnAuthorizationCodeReceived = async (context) =>
        {
            var processAuthorizationCode = await WoltersKluwerIdentityService.ProcessAuthorizationCode(
                context,
                woltersKluwerIdentityOptions).ConfigureAwait(true);
            
            context.Principal = processAuthorizationCode.ClaimsPrincipal;
            
            context.Success();
        },
        OnRedirectToIdentityProvider = (context) => Task.CompletedTask,
        OnRedirectToIdentityProviderForSignOut = (context) => Task.CompletedTask,
        OnAuthenticationFailed = (context) => Task.CompletedTask
    };
    
    public void Handler(OpenIdConnectOptions options)
    {

        options.Authority = woltersKluwerIdentityOptions.Authority;
        options.ClientId = woltersKluwerIdentityOptions.ClientId;
        options.ClientSecret = woltersKluwerIdentityOptions.ClientSecret;
        options.SignedOutRedirectUri = $"{woltersKluwerIdentityOptions.RedirectUrl}";
        options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.RequireHttpsMetadata = false;
        options.SaveTokens = true;
        options.Scope.Clear();
        options.ClaimActions.MapAll();
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.Events = _events;

        foreach (var scope in woltersKluwerIdentityOptions.Scopes)
        {
            options.Scope.Add(scope);
        }
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // NameClaimType = WoltersKluwerIdentityClaimTypes.GivenName,
            // AuthenticationType = woltersKluwerIdentityOptions.AuthenticationType,
            // SaveSigninToken = true
        };
        
        options.ForwardDefaultSelector = context =>
            IsPossibleBearer(context)
                ? JwtBearerDefaults.AuthenticationScheme
                : OpenIdConnectDefaults.AuthenticationScheme;
        
    }

    private static bool IsPossibleBearer(HttpContext context)
    {
        return !context.Request.Path.StartsWithSegments("/UserAuth") && 
               (
                   context.Request.Path.StartsWithSegments("/api") || 
                   context.Request.Headers.ContainsKey(WoltersKluwerIdentityAuthenticationConstants.BearerAuthorizationHeaders) || 
                   context.Request.Query.ContainsKey(WoltersKluwerIdentityClaimTypes.AccessToken)
               );
    }
    
}