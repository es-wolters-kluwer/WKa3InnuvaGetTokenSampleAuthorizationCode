using System.Security.Claims;
using IdentityModel.Client;

namespace WKa3innuvaGetTokenSample.Authentication.WoltersKluwerIdentity;

public class ProcessAuthorizationCodeResult
{
    public required ClaimsPrincipal ClaimsPrincipal { get; set; }

    /// <summary>
    /// Gets or sets the access token response.
    /// </summary>
    /// <value>
    /// The access token response.
    /// </value>
    public required TokenResponse AccessTokenResponse { get; set; }
}