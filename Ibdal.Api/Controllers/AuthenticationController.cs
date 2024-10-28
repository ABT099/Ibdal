using System.Security.Claims;
using Ibdal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginForm loginForm,
        [FromServices] AppDbContext ctx,
        [FromServices] IUserClaimsPrincipalFactory<IdentityUser> claimsPrincipalFactory,
        [FromServices] SignInManager<IdentityUser> signInManager,
        [FromServices] UserManager<IdentityUser> userManager)
    {
        var user = await userManager.FindByNameAsync(loginForm.Username);
        if (user is null) return NotFound();

        var result = await signInManager.CheckPasswordSignInAsync(user, loginForm.Password, false);
        if (result.Succeeded == false) return BadRequest("Invalid username or password.");

        var principal = await claimsPrincipalFactory.CreateAsync(user);
        var identity = principal.Identities.First();

        var id = await ctx.Users
                     .Find(x => x.AuthId == user.Id)
                     .Project(x => x.Id)
                     .FirstOrDefaultAsync()
                 ?? await ctx.Stations
                     .Find(x => x.AuthId == user.Id)
                     .Project(x => x.Id)
                     .FirstOrDefaultAsync();

        if (id is null)
            return BadRequest("Invalid username or password.");

        var roles = await userManager.GetRolesAsync(user);

        if (roles.Contains("Admin"))
            identity.AddClaim(new Claim(Constants.Claims.Role, Constants.Roles.Admin));
        else if (roles.Contains("Station"))
            identity.AddClaim(new Claim(Constants.Claims.Role, Constants.Roles.Station));
        else if (roles.Contains("Customer"))
            identity.AddClaim(new Claim(Constants.Claims.Role, Constants.Roles.Customer));
        else
            return Unauthorized();

        identity.AddClaim(new Claim(Constants.Claims.Id, id));

        var key = new RsaSecurityKey(Constants.Keys.RsaKey);

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = "https://Ibdal.com",
            Subject = identity,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
        });

        return Ok(token);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordForm changePasswordForm,
        [FromServices] UserManager<IdentityUser> userManager)
    {
        var user = await userManager.FindByIdAsync(changePasswordForm.AuthId);

        if (user is null) return NotFound();

        var result =
            await userManager.ChangePasswordAsync(user, changePasswordForm.OldPassword, changePasswordForm.NewPassword);

        if (result.Succeeded == false) return BadRequest();

        return Ok();
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorizationHeader) ||
            !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();

        var token = authorizationHeader["Bearer ".Length..].Trim();

        if (string.IsNullOrWhiteSpace(token)) return BadRequest("Invalid token.");

        var hashedToken = HashToken.Invoke(token);
        Constants.BlackList.Add(hashedToken);

        return Ok();
    }
}