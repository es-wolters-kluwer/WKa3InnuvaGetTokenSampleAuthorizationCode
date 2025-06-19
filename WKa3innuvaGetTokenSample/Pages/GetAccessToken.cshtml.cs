using System.Globalization;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WKa3innuvaGetTokenSample.Authentication.WoltersKluwerIdentity;

namespace WKa3innuvaGetTokenSample.Pages;

[Authorize]
public class GetAccessTokenModel : PageModel
{
    
    private const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'GMT'K";
    
    public string ClientId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string AccessTokenExpiration { get; set; } = string.Empty;
    public string AuthenticationDateTime { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string WkIdCda { get; set; } = string.Empty;
    public string WkUserId { get; set; } = string.Empty;
    public string OtherInfo { get; set; } = string.Empty;

    public void OnGet()
    {
        //Show Oauth access-token, refresh-token and claims to the user.
        ClientId = User.FindFirstValue(JwtClaimTypes.Audience)!;
        AccessToken = User.FindFirstValue(WoltersKluwerIdentityClaimTypes.AccessToken)!;
        RefreshToken = User.FindFirstValue(WoltersKluwerIdentityClaimTypes.RefreshToken)!;
        Name = User.FindFirstValue(WoltersKluwerIdentityClaimTypes.DisplayName)!;
        AccessTokenExpiration = DateTimeOffset.FromUnixTimeSeconds(
            (long)Convert.ToDouble(User.FindFirstValue(JwtClaimTypes.Expiration)))
            .DateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
        AuthenticationDateTime = DateTimeOffset.FromUnixTimeSeconds(
            (long)Convert.ToDouble(User.FindFirstValue(JwtClaimTypes.AuthenticationTime)))
            .DateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
        WkUserId = User.FindFirstValue(WoltersKluwerIdentityClaimTypes.BackOfficeId)!;
        WkIdCda = User.FindFirstValue(WoltersKluwerIdentityClaimTypes.Tenant)!;
        
        foreach(var claim in User.Claims)
        {
            if (claim.Type == "scope")
            {
                if (claim.Value == "WK.ES.A3EquipoContex")
                    OtherInfo = String.Format("SecondUserId:{0}, UserId:{1}", User.FindFirstValue("wk.es.secondclientid"), User.FindFirstValue("wk.es.a3equipouserid"));
                if (claim.Value == "WK.ES.NEWPOL.COR.API")
                    OtherInfo = String.Format("wk.es.keyusercorrelationid:{0}, Modules:{1}", User.FindFirstValue(WoltersKluwerIdentityClaimTypes.KeyUserCorrelationId), User.FindFirstValue(WoltersKluwerIdentityClaimTypes.Modules));
            }
        }
    }

    public async Task<IActionResult> OnPostLogout()
    {            
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);            
        return Redirect("~/");
    }
}