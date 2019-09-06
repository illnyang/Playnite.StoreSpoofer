using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using StoreSpoofer.Extensions;

namespace StoreSpoofer
{
    public class StoreSpooferPlugin : Plugin
    {
        private ILogger _logger;
        private readonly IPlayniteAPI _api;

        private readonly Dictionary<string, Guid> _libraryNameToGuid = new Dictionary<string, Guid>
        {
            ["None (Playnite)"] = Guid.Empty,
            ["BattleNet"] = Guid.Parse("E3C26A3D-D695-4CB7-A769-5FF7612C7EDD"),
            ["Bethesda"] = Guid.Parse("0E2E793E-E0DD-4447-835C-C44A1FD506EC"),
            ["Epic"] = Guid.Parse("00000002-DBD1-46C6-B5D0-B1BA559D10E4"),
            ["Gog"] = Guid.Parse("AEBE8B7C-6DC3-4A66-AF31-E7375C6B5E9E"),
            ["Itchio"] = Guid.Parse("00000001-EBB2-4EEC-ABCB-7C89937A42BB"),
            ["Origin"] = Guid.Parse("85DD7072-2F20-4E76-A007-41035E390724"),
            ["Steam"] = Guid.Parse("CB91DFC9-B977-43BF-8E70-55F46E410FAB"),
            ["Twitch"] = Guid.Parse("E2A7D494-C138-489D-BB3F-1D786BEEB675"),
            ["Uplay"] = Guid.Parse("C2F038E5-8B92-4877-91F1-DA9094155FC5")
        };

        private readonly Dictionary<Guid, Guid> _gameGuidToOldLibraryGuid = new Dictionary<Guid, Guid>();
        private readonly Dictionary<Guid, string> _gameGuidToOldGameId = new Dictionary<Guid, string>();

        public override Guid Id { get; } = nameof(StoreSpooferPlugin).ToGuid();

        public StoreSpooferPlugin(IPlayniteAPI api) : base(api)
        {
            _api = api;
            _logger = api.CreateLogger();
        }

        public override IEnumerable<ExtensionFunction> GetFunctions()
        {
            return new List<ExtensionFunction>
            {
                new ExtensionFunction("Change Game Id of the Selected Game", () =>
                {
                    var count = _api.MainView.SelectedGames.Count();

                    if (count == 1)
                    {
                        var game = _api.MainView.SelectedGames.Single();

                        var enterGameIdDialogResult = _api.Dialogs.SelectString("New Game Id",
                            $"Changing Game Id of {game.Name}", game.GameId);

                        if (enterGameIdDialogResult.Result)
                        {
                            if (!_gameGuidToOldGameId.ContainsKey(game.Id))
                            {
                                _gameGuidToOldGameId[game.Id] = game.GameId;
                            }
                            
                            _api.Database.Games.Single(x =>
                            {
                                if (x.Id != game.Id)
                                    return false;

                                x.GameId = enterGameIdDialogResult.SelectedString;
                                return true;
                            });
                        }
                    }
                    else if (count > 1)
                    {
                        _api.Dialogs.ShowErrorMessage(
                            "You cannot edit Game Id of multiple Games at once. Please select single Game.",
                            "Please select single Game.");
                    }
                    else
                    {
                        _api.Dialogs.ShowErrorMessage("Please select a Game.", "No Game selected.");
                    }
                }),
                new ExtensionFunction("Restore Game Id of the Selected Game(s)", () =>
                {
                    if (!_api.MainView.SelectedGames.Any())
                    {
                        _api.Dialogs.ShowErrorMessage("Please select at least one Game.", "No Games selected.");
                        return;
                    }

                    foreach (var game in _api.MainView.SelectedGames)
                    {
                        if (_gameGuidToOldGameId.ContainsKey(game.Id))
                        {
                            game.GameId = _gameGuidToOldGameId[game.Id];
                        }
                    }
                }),
                new ExtensionFunction("Change Library Plugin of the Selected Game(s)", () =>
                {
                    if (!_api.MainView.SelectedGames.Any())
                    {
                        _api.Dialogs.ShowErrorMessage("Please select at least one Game.", "No Games selected.");
                        return;
                    }

                    var messageBoxTextBuilder = new StringBuilder();
                    messageBoxTextBuilder.Append("Available plugins: ");
                    messageBoxTextBuilder.Append(string.Join(", ", _libraryNameToGuid.Select(x => x.Key)));
                    messageBoxTextBuilder.Append(".");

                    showDialog:
                    var enterGameIdDialogResult = _api.Dialogs.SelectString(messageBoxTextBuilder.ToString(),
                        "Enter new Library Plugin name", "None");

                    if (!enterGameIdDialogResult.Result)
                    {
                        return;
                    }

                    if (!_libraryNameToGuid.ContainsKey(enterGameIdDialogResult.SelectedString))
                    {
                        _api.Dialogs.ShowErrorMessage(
                            "Given Library Plugin does not exist. Please enter a correct name or cancel.",
                            "Failed to find corresponding Guid.");
                        goto showDialog;
                    }

                    foreach (var game in _api.MainView.SelectedGames)
                    {
                        if (!_gameGuidToOldLibraryGuid.ContainsKey(game.Id))
                        {
                            _gameGuidToOldLibraryGuid[game.Id] = game.PluginId;
                        }

                        game.PluginId = _libraryNameToGuid[enterGameIdDialogResult.SelectedString];
                    }
                }),
                new ExtensionFunction("Restore Library Plugin of the Selected Game(s)", () =>
                {
                    if (!_api.MainView.SelectedGames.Any())
                    {
                        _api.Dialogs.ShowErrorMessage("Please select at least one Game.", "No Games selected.");
                        return;
                    }

                    foreach (var game in _api.MainView.SelectedGames)
                    {
                        if (_gameGuidToOldLibraryGuid.ContainsKey(game.Id))
                        {
                            game.PluginId = _gameGuidToOldLibraryGuid[game.Id];
                        }
                    }
                })
            };
        }
    }
}