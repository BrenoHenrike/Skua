## About Skua
Skua is the successor to RBot (originally made by "[rodit](https://github.com/rodit/RBot)"), now remade and rebranded by Breno_Henrike, with help from Delfina, Lord Exelot, and a handful of scripters. It is a Third Party Client made by the people mentioned above. It also has many "features" and quirks. Overall it will make this glorified flash game on steroids a piece of cake.

## Do we store information online?
The *only* things that get recorded are; auto-generated number **(not your actual game user ID)** to identify you, the amount of scripts ran (stopped & started), and for the start and stop timestamps. This can be completely opted out of when first running a script, or you can edit the text file ***“DataCollectionSettings”*** in your `Documents\Skua > DataCollectionSettings.txt`. If you make it look as shown below, it will send absolutely nothing 👍
```
UserID: null
genericDataConsent: false
scriptNameConsent: false
stopTimeConsent: false
```

## For Account Manager
Your **Account Info** will be stored only in your **local appdata** and never shown anywhere nor in a text file. We **DO NOT** store it online because we intended to make account manager with **no database**.

## What do we use this data for?
To keep track of what bots are run, how often, how long, and really just how popular some bots are.

## Some example of the types of scripts Skua has:
- **Story scripts** found in the `Story` folder.
- **Merge scripts** found in the `Other > MergeShops` folder.
- **Farming scripts** found in the `Farm` folder. These include but are not limited by: Gold, Experience, Class Points, Reputation.
- **Faction-specific** (nation/legion/etc) can be found in their respective folders.
- Specific tools such as **Butler** (a follow and kill [doesnt support quests]), "ChooseBestGear" (a script that will look at your inv, and equip the appropriate setting for the race type you select.), BuyOut ( will either buy **all/non-ac/ac** (will prompt due to acs) from a specified shop)
- **Core Script Files** are not meant to be run.
- **0ScripName.cs** are basically "Do everything required for this script.."
-  If you wanted to have a new farming script that they don't exist though please request them [here](https://forms.gle/casF8pCNsP2qMGZS6)

## How to use:
In order for Skua to run *correctly*, you will need to download and install the [Skua Dependencies](https://github.com/BrenoHenrike/Skua/releases/download/1.0.0.0/Skua.Dependencies.exe) (fetch the correct zip for your PC, most people will want the x64 version.) Install the Dependencies and open `Skua Manager`. Once it's opened, you can manage your accounts in account manager and start the Skua client with `Start All` or `Start Selected` button. Once Skua is open, you will get the **Script Updates** popup. After that you can start sailing.

## Skua Features:
- **Account Manager**; allows you to have multiple accounts and switch between them. You can even have multiple clients open at once.
- Skua automatically **Downloads & Updates** scripts for you; no more hassle of finding them on the site as Grimoire does.
- It notifies you if client updates are available, and has an option to do the same for pre-releases.
- We scripts for 99% of the farms ingame; the only thing excluded at the moment is ultras; It is currently in works but is still in its early stages.
- There is a good team to fix bugs- and client-issues.  And to fix or create scripts upon request; scripts can be requested *assuming they don't exist yet* [here](https://forms.gle/casF8pCNsP2qMGZS6)*
- **Get scripts** will re-fetch the scripts upon clicking "update all", if none are there hit "download all. It also has a search bar which allows you to easily find scripts.
- **Load scripts** will navigate you to the `Scripts Folder` for you to explore, and find the script you want.
- **Edit scripts** is there if you want to edit something in the currently loaded script. ***not recommended for non-scripters***
- **Start Script** will start the script you have loaded.
- **Helpers Tab** 
    - **Runtime** allows you to register a quest to be continually be accepted and turned in, drops to be picked up, or Booster automatically used.
    - **Fast Travel** in the top right you can select private and specify a room number if preferred (it's very highly suggested so people don't see you teleporting between cells). You can then click a button to Teleport directly there (for /tercessuinotlim teleports you may need to press - wait - then press again.)
- **Tool Tab**
    - **Current Drops**; Drops in AQW when you decline them are never *truly* gone until you log out, that's what this helper is for, did you delete something? Did the script decline something you want? Well open this and it'll be there (unless you logged out or disconnected..)
    - **Loader**; Loads the Quests.txt file, for Quest names & IDs (can use the search function), or when you find the one you want, load it. Also has a shop loader (IDs can be found in the discord in either the [#quest-ids](https://discord.com/channels/1008293278162092073/1042872458421739612) or [#shop-Ids](https://discord.com/channels/1008293278162092073/1042877939236225154) channel)
    - **Grabber** can “grab” multiple things from game data, including; Shop Items/IDs, Quest Data, Inventory(temp/house/bank), Monsters (cell and map wide), and MapItemIds (those items you need to click on around the map for quests.
    - **Stats** shows current stats, for example: Kills, Deaths, Quests(accepted/completed), Pickups, and time since you opened the client or  have last reset the stats.
    - **Console** shows the console, and allows you to type in commands, and see the output. ***(For devs only)***
- **Skills Tab**
    - Allows you to edit and create custom skillsets (besides the ones we already provide for you.), if you have skillset suggestions (bot skill #'s are a number less than ingame, so for example in-game "skill 2-3-4-5" in bot terms "skill 1-2-3-4", potions/consumables and auto-attack (0 and 6) are not useable.
    - We have already made a lot of combos for almost every class the game has to offer, so the bot will work with that right from the start.
- **Packets Tab**
    - **Spammer** will send a packet you grabbed from the logger to either the client (make sure that's checked if you want) or the server (uncheck client) 
    - **Logger**; Click enable (disabled by default), and it will Log Packets the game sends to the server.
    - **Interceptor**; Similair to the logger but also includes packets the server sends to the client. It also needs to be hooked to the server, so it's recommended to do it from the login screen. Open the interceptor, select the server from the dropdown menu and hit connect. It will log in the last logged in account / what is saved.
- **Bank Tab**
    - Opens your bank (as if you clicked on a bank-pet) and can be used *anywhere*
- **Logs Tab**
    - Logs what the bot is doing, this is also shown in the "Scripts" button window.
- **Plugins Tab** 
    - Where you can load `*.dll` files that follows the Skua Plugin Template. Or even existing skua plugin from the [Skua Discord](https://discord.gg/hstRRK9G7w).
- **Auto Tab**
    - **Auto-Attack** will automatically attack any of the monster in the current cell.
    - **Auto-Hunt** will automatically hunt the selected monster across the entire map.
        - ***How to use:*** Click the monster in game to select it. Then start Auto-Hunt. The bot will jump across the map to kill all instances of your selected monster. If no monster is selected before starting Auto-Hunt, it will consider all monsters in your current cell as selected and will hunt all of those in stead.
    - **Class use mode** Press the refresh icon to reload the list of your class and skill mode to select. Then click "use select", and then hit either button to start. The Auto-Attack or Auto-Hunt will then use that skillset for its abilities.

### For questions or help join the discord & go to the [#skua-chat](https://discord.com/channels/1078578570026291262/1078663021024510004) channel.

### [Skua Discord](https://discord.gg/hstRRK9G7w) join the community and get help with Skua.

# Sponsorship & Donation ❤️
For us to make the Skua better, skua developers need your support. You can support us by donating or sponsor us by clicking the PayPal link below. Thank you for your support.
### Skua Developers
- [Breno Henrike's PayPal](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY).
- [Yor Delfina's PayPal](https://www.paypal.com/donate/?hosted_button_id=DMZFDRYJ5BT96)
- [Lord Exelot's PayPal](https://www.paypal.me/LordExelot)

## Casts:
- **Breno Henrike** the artist of Skua.
- **Delfina** the developer of Skua.
- **Lord Exelot** the scripts manager and Skua Discord owner.
- **tato** the scripts writer, also the writer of this about content.
- **Skua Heroes** the script makers and helpers.
- **Boaters** the ones who sail overnight using Skua and help the Skua team to improve, thanks to their feedback and suggestions **which is you**.
