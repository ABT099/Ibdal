using Microsoft.AspNetCore.Identity;

namespace Ibdal.Api.Services;

public class AuthService(UserManager<IdentityUser> uMgr)
{
    public async Task<string?> CreateUser(string userName, string password)
    {
        var user = await uMgr.FindByNameAsync(userName);

        if (user != null)
            return null;

        user = new IdentityUser(userName);

        var createResult = await uMgr.CreateAsync(user, password);

        if (!createResult.Succeeded)
            return null;

        return user.Id;
    }
}