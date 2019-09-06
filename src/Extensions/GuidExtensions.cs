using System;
using System.Text;

namespace StoreSpoofer.Extensions
{
    internal static class GuidExtensions
    {
        public static Guid ToGuid(this string src)
        {
            var stringBytes = Encoding.UTF8.GetBytes(src);
            var hashedBytes = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(stringBytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }

        public static Guid ToGuid(this GameLibrary library)
        {
            return AvailablePlugins.LibraryToGuid[library];
        }
    }
}