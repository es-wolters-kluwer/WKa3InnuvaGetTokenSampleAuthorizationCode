using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace WKa3innuvaGetTokenSample.Authentication.WoltersKluwerIdentity;

public static class WoltersKluwerIdentityService
{

    private const string WellKnownEndpoint = ".well-known/openid-configuration";
    
    public static async Task<ProcessAuthorizationCodeResult> ProcessAuthorizationCode(AuthorizationCodeReceivedContext context, WoltersKluwerIdentityOptions woltersKluwerIdentityOptions)
    {
        
        var request = context.HttpContext.Request;
        var currentUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path);
        var authorizationRequest = new AuthorizationCodeTokenRequest
        {
            RedirectUri = currentUri,
            ClientId = woltersKluwerIdentityOptions.ClientId,
            ClientSecret = woltersKluwerIdentityOptions.ClientSecret,
            Address = woltersKluwerIdentityOptions.TokenEndpoint,
            Code = context.ProtocolMessage.Code,
            ClientCredentialStyle = ClientCredentialStyle.PostBody
        };
        
        using var httpClient = new HttpClient();
        var tokenResponse = await httpClient.RequestAuthorizationCodeTokenAsync(authorizationRequest).ConfigureAwait(true);
        
        var userInfoRequest = BuildUserInfoRequest(tokenResponse, woltersKluwerIdentityOptions); 
        var userInfoResponse = await httpClient.GetUserInfoAsync(userInfoRequest).ConfigureAwait(true);
        var claimsPrincipal = GetClaimsPrincipal(tokenResponse, woltersKluwerIdentityOptions);
        
        claimsPrincipal.AddIdentity(new ClaimsIdentity(userInfoResponse.Claims.ToArray()));
        claimsPrincipal.AddIdentity(new ClaimsIdentity([
            new Claim(WoltersKluwerIdentityClaimTypes.AccessToken, tokenResponse.AccessToken),
            new Claim(WoltersKluwerIdentityClaimTypes.RefreshToken, tokenResponse.RefreshToken)
        ]));
        
        context.HandleCodeRedemption(tokenResponse.AccessToken, context.ProtocolMessage.IdToken);

        return new ProcessAuthorizationCodeResult()
        {
            ClaimsPrincipal = claimsPrincipal,
            AccessTokenResponse = tokenResponse,
        };
        
    }

    private static UserInfoRequest BuildUserInfoRequest(TokenResponse tokenResponse,
        WoltersKluwerIdentityOptions woltersKluwerIdentityOptions) =>
        new UserInfoRequest
        {
            Address = $"{woltersKluwerIdentityOptions.Authority}connect/userinfo",
            Token = tokenResponse.AccessToken
        };

    private static ClaimsPrincipal GetClaimsPrincipal(TokenResponse tokenResponse, WoltersKluwerIdentityOptions woltersKluwerIdentityOptions)
    {

        var wellKnownUrl = $"{woltersKluwerIdentityOptions.Authority}{WellKnownEndpoint}";
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(wellKnownUrl, new OpenIdConnectConfigurationRetriever());
        var optOpenIdConnectConfiguration = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuers = woltersKluwerIdentityOptions.ValidIssuers.Split(" "),
            ValidAudiences = woltersKluwerIdentityOptions.ValidAudiences.Split(" "),
            IssuerSigningKeys = optOpenIdConnectConfiguration.SigningKeys
        };
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = jwtTokenHandler.ValidateToken(tokenResponse.AccessToken, validationParameters, out var securityToken);

        return claimsPrincipal;
        
    }


}