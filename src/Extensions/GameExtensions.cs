using Playnite.SDK.Models;

namespace StoreSpoofer.Extensions
{
    internal static class GameExtensions
    {
        public static GameLibrary GetLibraryType(this Game game)
        {
            return game.PluginId.ToGameLibrary();
        }
    }
}