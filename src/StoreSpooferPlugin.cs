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
                    messageBoxTextBuilder.Append(string.Join(", ", AvailablePlugins.LibraryToGuid.Select(x => x.Key.EnumGetDescription())));
                    messageBoxTextBuilder.Append(".");

                    showDialog:
                    var enterGameIdDialogResult = _api.Dialogs.SelectString(messageBoxTextBuilder.ToString(),
                        "Enter new Library Plugin name", "None");

                    if (!enterGameIdDialogResult.Result)
                    {
                        return;
                    }

                    GameLibrary userResult;

                    var selectedString = enterGameIdDialogResult.SelectedString;

                    if (string.Equals(selectedString, "playnite", StringComparison.InvariantCultureIgnoreCase))
                    {
                        userResult = GameLibrary.None;
                    }
                    else if (!selectedString.EnumFromDescription(out userResult) && !Enum.TryParse(selectedString, true, out userResult))
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

                        game.PluginId = userResult.ToGuid();
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