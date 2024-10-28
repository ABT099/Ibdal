namespace Ibdal.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected string Id => GetClaim(Constants.Claims.Id);
    protected string Role => GetClaim(Constants.Claims.Role);
    
    private string GetClaim(string claimType) => HttpContext.User.Claims
        .FirstOrDefault(x => x.Type.Equals(claimType))?.Value!;
}