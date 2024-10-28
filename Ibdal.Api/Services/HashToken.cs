using System.Security.Cryptography;
using System.Text;

namespace Ibdal.Api.Services;

public static class HashToken
{
    public static string Invoke(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var hashString = Convert.ToBase64String(hash);
        return hashString;
    }
}