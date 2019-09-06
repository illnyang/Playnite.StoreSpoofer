using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace StoreSpoofer
{
    public enum GameLibrary
    {
        [Description("None (Playnite)")]
        None,
        
        [Description("Battle.net")]
        BattleNet,
        
        [Description("Bethesda")]
        Bethesda,
        
        [Description("Epic")]
        Epic,
        
        [Description("GOG")]
        Gog,
        
        [Description("itch.io")]
        Itchio,
        
        [Description("Origin")]
        Origin, 
        
        [Description("Steam")]
        Steam,
        
        [Description("Twitch")]
        Twitch,
        
        [Description("Uplay")]
        Uplay,
    }

    public static class AvailablePlugins
    {
        private const string BattleNetGuid = "E3C26A3D-D695-4CB7-A769-5FF7612C7EDD";
        private const string BethesdaGuid = "0E2E793E-E0DD-4447-835C-C44A1FD506EC";
        private const string EpicGuid = "00000002-DBD1-46C6-B5D0-B1BA559D10E4";
        private const string GogGuid = "AEBE8B7C-6DC3-4A66-AF31-E7375C6B5E9E";
        private const string ItchioGuid = "00000001-EBB2-4EEC-ABCB-7C89937A42BB";
        private const string OriginGuid = "85DD7072-2F20-4E76-A007-41035E390724";
        private const string SteamGuid = "CB91DFC9-B977-43BF-8E70-55F46E410FAB";
        private const string TwitchGuid = "E2A7D494-C138-489D-BB3F-1D786BEEB675";
        private const string UplayGuid = "C2F038E5-8B92-4877-91F1-DA9094155FC5";
        
        public static readonly Dictionary<GameLibrary, Guid> LibraryToGuid = new Dictionary<GameLibrary, Guid>
        {
            [GameLibrary.None] = Guid.Empty,
            [GameLibrary.BattleNet] = Guid.Parse(BattleNetGuid),
            [GameLibrary.Bethesda] = Guid.Parse(BethesdaGuid),
            [GameLibrary.Epic] = Guid.Parse(EpicGuid),
            [GameLibrary.Gog] = Guid.Parse(GogGuid),
            [GameLibrary.Itchio] = Guid.Parse(ItchioGuid),
            [GameLibrary.Origin] = Guid.Parse(OriginGuid),
            [GameLibrary.Steam] = Guid.Parse(SteamGuid),
            [GameLibrary.Twitch] = Guid.Parse(TwitchGuid),
            [GameLibrary.Uplay] = Guid.Parse(UplayGuid)
        };

        public static readonly Dictionary<Guid, GameLibrary> GuidToLibrary = new Dictionary<Guid, GameLibrary>
        {
            [Guid.Empty] = GameLibrary.None,
            [Guid.Parse(BattleNetGuid)] = GameLibrary.BattleNet,
            [Guid.Parse(BethesdaGuid)] = GameLibrary.Bethesda,
            [Guid.Parse(EpicGuid)] = GameLibrary.Epic,
            [Guid.Parse(GogGuid)] = GameLibrary.Gog,
            [Guid.Parse(ItchioGuid)] = GameLibrary.Itchio,
            [Guid.Parse(OriginGuid)] = GameLibrary.Origin,
            [Guid.Parse(SteamGuid)] = GameLibrary.Steam,
            [Guid.Parse(TwitchGuid)] = GameLibrary.Twitch,
            [Guid.Parse(UplayGuid)] = GameLibrary.Uplay
        };
    }
}