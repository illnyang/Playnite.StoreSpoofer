# Playnite.StoreSpoofer

This plugin allows for per-entry editing of `GameId` and `PluginId` properties of your games.
Here are some scenarios in which you'd want to edit  those properties:

 - Download metadata from an official store for a manually imported game.
 - Use metadata from a different store than the game was initially imported from.

Includes an option to mass-undo all the changes you've made to your database with library.

# Installation

Go to `%localappdata%\Playnite\Extensions` (or wherever you've installed your Playnite instance) and create a folder for the plugin. You can name this folder however you prefer, the assembly will load regardless. Copy all files from `/bin/${Configuration}/` (or root dir of the archive, in you downloaded the plugin from Github's releases) to this folder and restart your Playnite process.

# Usage

After you've installed the plugin, its functions will be listed in the `Playnite->Extensions` menu.
Here's a step-by-step guide on how to download metadata from a given store.

 0. Find proper GameId for your Game with correspondence to the store you plan to use. Consult `Finding GameId` subsection for more insight.
 1. Use `Change Game Id of the Selected Game` and enter the new GameId.
 2. Use `Change Library Plugin of the Selected Game(s)` and enter the name of the new Store.
 3. Download metadata using Playnite's built-in editor - `Official Store` option should now be available.
 4. Use `Restore Library Plugin of the Selected Game(s)` and `enter code here`

Please note that some functions support multi-select editing, make use of that as you will.

## Finding GameId
Internally, Playnite passes `GameId` property to the corresponding plugin's `MetadataProvider` implementation, thus there is no universal way of obtaining it for each and every game store. Consult [Playnite's source code](https://github.com/JosefNemec/Playnite/tree/master/source/Plugins) for more info.

| Provider | Protips |
|----------|---------|
| BattleNet | [Copy 'ProductId' field from here](https://github.com/JosefNemec/Playnite/blob/master/source/Plugins/BattleNetLibrary/BattleNetGames.cs) |
| Bethesda | See below |
| Epic | See below |
| GOG | [Just copy the ID from here](https://gogapidocs.readthedocs.io/en/latest/gameslist.html) |
| Itchio | See below |
| Origin | See below |
| Steam | [The number in this URL is the ID](https://store.steampowered.com/app/1092660/Blair_Witch/).
| Twitch | See below |
| Uplay | See below

****

**Bethesda**  
The last part of the url is the id, but that does not matter because of [this code](https://github.com/JosefNemec/Playnite/blob/master/source/Plugins/BethesdaLibrary/BethesdaMetadataProvider.cs#L17-L21). This cannot be mitigated unless I implement runtime IL patching sometime in the future or ship patched plugin assembly. Not that it's worth it anyway - it doesn't download any interesting stuff.

****

**Epic**  
Actually, you don't need any sort of ID to pull this off. [Go here](https://www.epicgames.com/store/en-US/) and copy the name to the `Name` field in the Playnite's editor. *You must uncheck* `Installed` *checkbox as well*.

****

**Itchio**  
Currently impossible to mitigate. [See this](https://github.com/JosefNemec/Playnite/blob/master/source/Plugins/ItchioLibrary/ItchioLibrary.cs#L150). Even if Butler supports grabbing all available games' ids and allows you to download their metadata, Playnite does not and it would require either a new MetadataProvider coded from scratch or heavy modifications to the code.

****

**Origin**  
Go to [Origin Store](https://www.origin.com/pol/en-us/store/the-sims/the-sims-4/) with Development Tools of your browser enabled. Look for the first API call that [looks like this](https://i.imgur.com/eBmnHvT.png). Highlighted part within quotes is the ID. It should be noted that sometimes the metadata will not download fully (for example: background is missing in the case of The Sims 4 but present with Battlefield 1). Unsure whether it is a bug, more investigation is needed.
*Remember to disable the* `Installed` *checkbox!*

****

**Twitch**  
Playnite uses Twitch's API to download metadata, so you need to own the game beforehand.

****

**Uplay**  
Same as Bethesda; [Playnite checks if the game is installed](https://github.com/JosefNemec/Playnite/blob/master/source/Plugins/UplayLibrary/UplayMetadataProvider.cs#L18-L22).

****

# Building

Download or clone the project, restore the packages and build the project using preferred solution configuration. All necessary manifest files will be copied to the `/bin/${Configuration}/` folder alongside the binary.

# Future
As of now (9/6/2019) [there is no way to introduce custom Metadata Providers](https://github.com/JosefNemec/Playnite/issues/417) . A more viable option than this workaround-tier plugin would be to implement a custom Metadata Provider that could search various game stores for a given title and download the data via HTML parsing. This could be implemented in four ways:

 1. Wait until 6.0 release of Playnite and implement it the 'official' way.
 2. Use Mono.Cecil/other trickery to replace the official implementation with a custom one at runtime. Pretty hard.
 3. Create a boilerplate Library Plugin with said Metadata Provider and use `Playnite.StoreSpoofer` to temporarily switch to it, download the metadata, unswitch.
 4. Implement it via a Generic Plugin, possibly with a custom GUI as Playnite's Dialogs API is pretty limiting.


# License (MIT)

    Copyright (c) 2019 kvdrrrrr
    
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
