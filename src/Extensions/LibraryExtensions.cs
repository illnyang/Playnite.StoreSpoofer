using System;

namespace StoreSpoofer.Extensions
{
    public static class LibraryExtensions
    {
        public static GameLibrary ToGameLibrary(this Guid guid)
        {
            return AvailablePlugins.GuidToLibrary[guid];
        }
    }
}