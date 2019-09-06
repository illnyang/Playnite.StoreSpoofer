using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BattleNetLibrary;
using BattleNetLibrary.Models;
using Newtonsoft.Json;
using Playnite.SDK.Models;

namespace StoreSpoofer
{
    internal class AutoMatch
    {
        private static string _steamCache = null;
        private static string _gogCache = null;

        public struct MatchResult
        {
            public GameLibrary Plugin;
            public string GameId;
        }

        public static async Task<List<MatchResult>> FindMatches(Game game)
        {
            var result = new List<MatchResult>();

            var findResult = await TryFindSteam(game);

            if (findResult.HasValue)
                result.Add(findResult.Value);

            findResult = await TryFindGog(game);
            
            if (findResult.HasValue)
                result.Add(findResult.Value);
            
            findResult = await TryFindBattleNet(game);
            
            if (findResult.HasValue)
                result.Add(findResult.Value);

            return result;
        }

        private static string SanitizeString(string str)
        {
            return new string(str.Where(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x)).ToArray()).Replace("  ",
                " ");
        }

        private static bool SanitizedCompare(string a, string b)
        {
            return string.Equals(SanitizeString(a), SanitizeString(b));
        }

        private static async Task<MatchResult?> TryFindSteam(Game game)
        {
            if (!string.IsNullOrEmpty(game.InstallDirectory) && Directory.Exists(game.InstallDirectory))
            {
                var files = Directory.GetFiles(game.InstallDirectory, "steam_appid.txt", SearchOption.AllDirectories);

                if (files.Any())
                {
                    // TODO: assuming there is only one steam_appid.txt in all directories is not a good idea
                    return new MatchResult
                    {
                        GameId = File.ReadAllText(files.First()),
                        Plugin = GameLibrary.Steam
                    };
                }
            }

            if (game.Links != null)
            {
                foreach (var link in game.Links)
                {
                    var match = Regex.Match(link.Url, @"store.steampowered.com\/app\/([0-9]+)");
                    if (match.Groups.Count == 2)
                    {
                        return new MatchResult
                        {
                            GameId = match.Groups[1].Value,
                            Plugin = GameLibrary.Steam
                        };
                    }
                }
            }

            if (_steamCache == null)
            {
                using (var client = new HttpClient())
                {
                    _steamCache = await client.GetStringAsync(@"http://api.steampowered.com/ISteamApps/GetAppList/v2");
                }
            }

            using (var reader = new JsonTextReader(new StringReader(_steamCache)))
            {
                uint lastAppId = 0;
                while (await reader.ReadAsync())
                {
                    if (reader.Value == null)
                    {
                        continue;
                    }

                    if (reader.TokenType == JsonToken.Integer)
                    {
                        lastAppId = Convert.ToUInt32(reader.Value);
                    }
                    else if (reader.TokenType == JsonToken.String)
                    {
                        // TODO: fuzzy matching?
                        if (SanitizedCompare(game.Name, (string) reader.Value))
                        {
                            return new MatchResult
                            {
                                GameId = lastAppId.ToString(),
                                Plugin = GameLibrary.Steam
                            };
                        }
                    }
                }
            }

            return null;
        }

        private static async Task<MatchResult?> TryFindGog(Game game)
        {
            if (!string.IsNullOrEmpty(game.InstallDirectory) && Directory.Exists(game.InstallDirectory))
            {
                var files = Directory.GetFiles(game.InstallDirectory, "goggame-*", SearchOption.AllDirectories);

                if (files.Any())
                {
                    return new MatchResult
                    {
                        GameId = Path.GetFileNameWithoutExtension(files.First()).Substring(8),
                        Plugin = GameLibrary.Gog
                    };
                }
            }

            if (game.Links != null)
            {
                foreach (var link in game.Links)
                {
                    var match = Regex.Match(link.Url, @"gog.com\/game\/.+");
                    if (!match.Success)
                        continue;

                    using (var client = new HttpClient())
                    {
                        var webPage = await client.GetStringAsync(link.Url);

                        var propertyStart =
                            webPage.IndexOf("cardProductId: ", StringComparison.InvariantCultureIgnoreCase);

                        return new MatchResult
                        {
                            GameId = webPage.Substring(propertyStart + 16, 10),
                            Plugin = GameLibrary.Gog
                        };
                    }
                }
            }

            if (_gogCache == null)
            {
                using (var client = new HttpClient())
                {
                    _gogCache = await client.GetStringAsync(
                        @"https://gogapidocs.readthedocs.io/en/latest/_sources/gameslist.rst.txt");
                }
            }

            var curLine = string.Empty;
            var firstEntryPassed = false;
            using (var reader = new StringReader(_gogCache))
            {
                while ((curLine = await reader.ReadLineAsync()) != null)
                {
                    if (!curLine.StartsWith("| "))
                        continue;

                    if (!firstEntryPassed)
                    {
                        firstEntryPassed = true;
                        continue;
                    }

                    var columnEnd = curLine.IndexOf(" | ", StringComparison.InvariantCultureIgnoreCase);
                    var gameName = curLine.Substring(2, columnEnd - 2).TrimEnd();

                    // TODO: fuzzy matching?
                    if (SanitizedCompare(gameName, game.Name))
                    {
                        return new MatchResult
                        {
                            GameId = curLine.Substring(columnEnd + 3, curLine.Length - columnEnd - 2 - 3),
                            Plugin = GameLibrary.Gog
                        };
                    }
                }
            }

            return null;
        }

        private static async Task<MatchResult?> TryFindBattleNet(Game game)
        {
            foreach (var bnetGame in BattleNetGames.Games)
            {
                if (SanitizedCompare(bnetGame.Name, game.Name))
                {
                    return new MatchResult
                    {
                        GameId = bnetGame.ProductId,
                        Plugin = GameLibrary.BattleNet
                    };
                }
            }

            return null;
        }
    }
}