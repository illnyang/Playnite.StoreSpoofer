using System;
using System.Text;

namespace StoreSpoofer.Extensions
{
    public static class GuidExtensions
    {
        public static Guid ToGuid(this string src)
        {
            var stringBytes = Encoding.UTF8.GetBytes(src);
            var hashedBytes = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(stringBytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }
    }
}