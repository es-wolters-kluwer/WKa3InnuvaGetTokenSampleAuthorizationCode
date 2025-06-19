using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WKa3innuvaGetTokenSample.Authentication.WoltersKluwerIdentity;

namespace WKa3innuvaGetTokenSample.Pages;

public class IndexModel(ILogger<IndexModel> logger, IOptions<WoltersKluwerIdentityOptions> woltersKluwerIdentityOptions) : PageModel
{

    public string Configuration => JsonSerializer.Serialize(woltersKluwerIdentityOptions.Value, new JsonSerializerOptions { WriteIndented = true });
    
    public void OnGet()
    {
    }
    
    
}