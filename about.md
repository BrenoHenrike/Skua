### About Skua
Skua is the successor to Rbot (originally made by "rodit"), now maintained and renamed by Breno_Henrike, with help from Delfina & Lord Exelot, and a handful of scripters. It is a Third Party Client Made by the people above. It also has many "features", and quirks, and overall will just make this Glorified flash game on steroids a piece of cake.

### Do we store information?
The *only* things that get recorded are; auto-generated number(Not your actual UserID) to identify you, the amount of scripts ran(stopped & started), which script was run, and for how long. This can be completely opted out of when first running a script, or you can edit the text file ***“DataCollectionSettings”*** in your `SkuaFolder > Skua_Modules > DataCollectionSettings.txt`, Make it look below. This will send absolutely nothing 👍
```
UserID: null
genericDataConsent: false
scriptNameConsent: false
stopTimeConsent: false
```

### What do we use this data for?
To keep track of what bots are run, how often, how long, and really just how popular some bots are.

### Types of Scripts Skua has:
- **Story scripts** found in the `Story Folder`.
- **Merge scripts** found in the `Other > MergeShops Folder`.
- **Farming scripts** found in the `Farm Folder`. such as gold, exp, etc.
- **Faction-specific** (nation/legion/etc) can be found in their folders.
- Specific tools such as **Butler** (a follow and kill [doesnt support quests]), "ChooseBestGear" (a script that will look at your inv, and equip the appropriate setting for the race type you select.), Buyout ( will either buy **all/non-ac/ac** (will prompt due to acs) from a specified shop)
- **Core & 0Files** aren't meant to be run, and 0files are basically "Do everything required for this script.."
-  if you wanted to have a new farming script that they don't exist though please request them [here](https://forms.gle/casF8pCNsP2qMGZS6)

### How to use:
To run *correctly*, you will need to download and install [Skua Dependencies](https://github.com/BrenoHenrike/Skua/releases/download/1.0.0.0/Skua.Dependencies.exe) (fetch the correct zip for your PC, most will want the x64 version.) install the Dependencies and open Skua. Manager. once it's opened, click the "GitHub AUthentication" button in the top right and go through the steps on the window that pops up. once that's finished, press the "launch Skua" button. once Skua is open, you will get the "skills loaded" popup (close that).

### Skua Features:
- Automatically **Download & Update** scripts for you; no more hassle of finding them on the site as grim does.
- Notify you if Updates are available and even pre-releases.
- Have Scripts for 99% of the farms ingame; the only thing excluded atm is ultras; in the works but still very, very, early alpha.
- Good Team to Fix bugs, and client issues, and fix/create scripts upon request; scripts can be requested *if they don't exist though, please request them [here](https://forms.gle/casF8pCNsP2qMGZS6)*
- **Get scripts** will re-fetch the scripts upon clicking "update all", if none are there hit "download all,
- **Load scripts** will navigate you to `Scripts folder` for you to explore, and find the script you want.
- **Edit scripts** if you want to edit something in the currently loaded script. ***not recommended for non-scripters***
- **Start Script** will start the script you have loaded.
- **Helpers Tab** 
    - **Runtime** allows you to register a quest, drop, or Booster to be continually accepted/picked up/used.
    - **Fast Travel** in the top right you can select private and specify a room number if preferred (it's very highly suggested so people don't see you teleporting between cells). You can then click a button to Teleport directly there (for Terreccuimptlim teleports you may need to press - wait - then press again.)
- **Tool Tab**
    - **Current Drops**; Drops in AQW when you decline them are never *truly* gone until you log out, thus what this helper is for, did you delete something? did the script decline something you want? well open this and it'll be there (unless you logged out or disconnected..)
    - **Loader**; Loads The Quests.Txt file, for Quest names & ids (can use the search function), or when you find the one you want, load it. Also has a shop loader (IDs can be found in the discord in either the [#quest-ids](https://discord.com/channels/1008293278162092073/1042872458421739612) or  [#shop-Ids](https://discord.com/channels/1008293278162092073/1042877939236225154) channel)
    - **Grabber** can “grab” multiple things from game data, including; Shop Items/Ids, Quest Data, Inventory(/temp/house/bank), Monsters (Cell and Map wide), and MapItemIds (those items you need to click on around the map for quests.
    - **Stats** shows current stats ex: Kills, deaths, Quests(accepted/completed), pickups, religions, and time since ya opened the client / reset the stats.
    - **Console** shows the console, and allows you to type in commands, and see the output. ***(For devs only)***
- **Skills Tab**
    - Allows you to edit  & create custom skillsets (besides the ones we already provide for you.), if you have skillset suggestions (bot skill #'s are a number less than ingame, so for example in-game "skill 2-3-4-5" in bot terms "skill 1-2-3-4", potions/consumables and auto-attack (0 and 6) are not useable.
- **Packets Tab**
    - **Spammer** will send a packet you grabbed from the logger to either the client (make sure that's checked if you want) or the server (uncheck client) 
    - **Logger**; Click enable (disabled by default), and it will Log Packets from and to the server.
    - **Intercepto**; The same as the logger but with *a lot*  more detail, also it needs to be hooked to the server so it's recommended to do it from the login screen. just select the interceptor, select the server from the dropdown, and hit connect it will log in the last acc / what is saved.
- **Bank Tab**
    - Opens your bank (sorta as if ya clicked a pet ) and can be used *anywhere*
- **Logs Tab**
    - logs what the bot is doing, this is also shown in the "Scripts" button window.
- **Plugins Tab** 
    - Where you can load `dll` files that follows the Skua Plugin Template. Or even existing skua plugin from [Skua Discord](https://discord.gg/pearlharbor).
- **Auto Tab**
    - **Auto-Attack** will automatically attack the monster in the current cell.
    - **Auto-Hunt** will automatically hunt the monster across the map.
    - ***How to use:***
        - Click mob to select what to attack/hunt or just start to kill everything available in that cell. On death, the respawn point is set to that cell and pad. Press the refresh icon to reload the list of your class and skill mode to select. Then click "use select", and then hit either button to start.

#### For Questions & help join the discord & go to the [#skua-questions-help](https://discord.com/channels/1008293278162092073/1008293280087289983) channel.

# [Skua Discord](https://discord.gg/pearlharbor) join the community and get help with Skua.