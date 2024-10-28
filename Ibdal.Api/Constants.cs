using System.Security.Cryptography;

namespace Ibdal.Api;

public struct Constants
{
    public struct Claims
    {
        public const string Id = nameof(Id);
        public const string Role = nameof(Role);
    }
    
    public struct Roles
    {
        public const string Admin = nameof(Admin);
        public const string Station = nameof(Station);
        public const string Customer = nameof(Customer);
    }
    
    public struct Policies
    {
        public const string Admin = nameof(Admin);
        public const string Station = nameof(Station);
        public const string Customer = nameof(Customer);
    }
    
    public static class Keys
    {
        private static readonly RSA Rsa;

        static Keys()
        {
            Rsa = RSA.Create();
            var privateKeyBytes = File.ReadAllBytes("key");
            Rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        }
        
        public static RSA RsaKey => Rsa;
    }
    
    public static readonly List<string> BlackList = [];
}