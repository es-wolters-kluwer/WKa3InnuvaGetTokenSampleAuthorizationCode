using System.ComponentModel.DataAnnotations;

namespace WKa3innuvaGetTokenSample.Authentication.WoltersKluwerIdentity;

public class WoltersKluwerIdentityOptions
{
    
    public const string SectionName = "Authentication:WoltersKluwer";
    
    [Required]
    public string Authority { get; set; } = string.Empty;

    [Required]
    public string AuthorizeEndpoint { get; set; } = string.Empty;

    [Required]
    public string TokenEndpoint { get; set; } = string.Empty;

    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    [Required]
    public string AuthenticationScopes { get; set; } = string.Empty;

    public string[] Scopes => string.IsNullOrEmpty(AuthenticationScopes) ? new string[] { } : AuthenticationScopes.Split("+");
    
    [Required]
    public string RedirectUrl { get; set; } = string.Empty;
    
    [Required]
    public string ValidIssuers { get; set; } = string.Empty;

    [Required]
    public string ValidAudiences { get; set; } = string.Empty;

    
}