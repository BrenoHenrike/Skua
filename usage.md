# Skua use Guide

## Installation & Setup

1. [.net 6 SDK (x64) installer](<https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.420-windows-x64-installer>)

2. [CleanFlash](<https://drive.google.com/file/d/1R0KrjAyHTz6KfcAp_zsRh2Mrv25J0PbI/view>) (and for it to be installed properly do this: [CleanFlash Installation](https://imgur.com/ztsLYZ1))

3. [Skua 1.2.4.0 Release](<https://github.com/BrenoHenrike/Skua/releases/download/1.2.4.0/Skua-1.2.4.0-Release-x64.msi>)

- Install both `.net 6 SDK` and `CleanFlash` before installing `Skua`.

## Once everything is properly installed and working

1. Open `Skua` or the `Skua Manager` (these will be placed on your desktop by default.)

2. Close the Changelog that appears, if Skua isnt showing its more then likely in the system tray (little `^` in the bottom right. just RightClick it and "show client").

3. Before you start using scripts it is strongly **recomended** to setup your `CoreBot` Options

    - Start by going to `Options > CoreBots` and set the Following:
        - Tab 1 (`Loadout`):
            - **Classes**:  `Solo/Farm Class` and the `Mode` (meaning things such as `Defence`, `Attack`, `Basic`, etc)
            - **Equipment**: `Solo/Farm Equipment` this will be equiped when scripts call the `Core.EquipClass();` function.

        - Tab 2 (`Options`):

            - Really the only things you should Edit here are:
            - **Room Number**
            - Toggle options such as:
                - **Antilag**
                - **Bank misc**
                - **BestGear** (BestGear no longer being used as it causes crashes so its diabled from the core).
                - **AutoEnh**
                - **Force Off MessageBoxes**: MessageBoxes that appear during gameplay are turned off (During april fools they will appear so be warned of that ^_^).
                - **Logger in Chat** will enable the script logging messages in the chat. (These are only seen by you.)
                - **Private Rooms**: This is by default and we **HIGHTLY** suggested you keep it on.
                - **Public on Difficult Parts**: This will put you into a public instance of a map its farming if enabled (only enable if you're having trouble on e.g. Difficult Parts)
                - **Should Rest**: This will make you reset inbetween combat. Not really necessary or useful.
                - **Map After stopping the Bot**: This will be the map you teleport to when the bot is finished, having this as `None` will stop in the map the bot finished in or you can have a custom map e.g. `Home`, `Yulgar`

        - Tab 3 (`Other`):
            - **Boosters**
                - Toggle the use of Boosters during scripts if needed.
            - **Nation Farms**
                - Toggle Sell Voucher of Nulgath if the voucher is not needed during scripts/farming things such as `Supplies to spin the wheel` it will sell the voucher when its not the item its going for.
                - Toggle Do Swindles Return during Supplies: Wether it should do the quest `Swindles Return` during scripts / farming methods such as `Supplies to spin the wheel`)
            - **Bludrut Brawl (PVP)**
                - This is deprecated (no longer used), as killing all mobs returns 10 trophies rather then a measily 2..or 3 (I do not remember).
            - **Bot Creatures Only**
                - **you will not use this unless told by a developer**

4. With Skua opened, you can now start using the scripts

    - To start a script you can do one of two ways:

        - (Recommended) Press `Get Scripts` (if no scripts appear press the `⟳` button) then you can search for / load a script from here.
        - Press `Load script`, (this will take you to the `Documents > Skua > Scripts` folder.

    - Once a script is loaded press `Start` on the `scripts` window.

## Features

1. `Scripts` Button:
   - **Explained Above!**

2. `Options` Button:

    - `Game Options`
        - These options are explain above in the `Once everything is properly installed` section.

    - `Application Options`
        - These Optons are for if u dont want your `Scripts` or `SkillSets` to be Update / reset if you've changed them.

    - `CoreBots`
        - These options are explain above in the `Once everything is properly installed` section.

    - `Application Theme`
        - This is where you can set the `Theme` of the Skua Windows.

    - `HotKeys`
        - This is where you can change the default hotkeys.

3. `Helpers`

    - `RunTime`
      - This is where you can:
        - **Manually** set up Quests to automatically be accepted and turned in
        - Add drops to the droplist, to be picked up
        - Toggle accepting of AC drops / rejecting things that *aren't* in the drop list.
        - Toggle usage of boosts (if you aren't currently running a script.)

    - `Fast Travel`
        - Add or Use Preset/Custom Made *Fast Travel points* to get Around AQW

    - `Current Drops`
        - This is where the Rejected Drops (as long as you haven't been Disconnected/Logged out) will be for you to pick up if you missed something you want.

4. `Tools`

    - `Loader`
        1. Load Shops (most require you to be in the required map for the shop.)
        2. Load/Search for quests

    - `Grabber` **Pressing `Grab` on any of these tabs will grab the information**

        1. **Shop Items**: Gets the item of the currently loaded shop
        2. **Shop IDs**: gets the ShopIDs of the shops that have been loaded *so far* this login session.
        3. **Quests**: shows the QuestData for the quests that have been loaded in the current session
        4. **Inventory**: All the current items in your `Inventory`
        5. **House Inventory**: All the current items in your `House Inventory`
        6. **Temp Inventory**: All the current items in your `TempInventory` (mostly Quest Items)
        7. **Bank Items**: All the current items in your `Bank`
        8. **Cell Monsters**: Data for All the Mobs in the current Cell (the room you're in)
        9. **Map Mosnters**:Data for All the Mobs in the current Map
        10. **GetMap Item IDs**: This gets the Quests Items that are clickable on the map. (may or may not work)

    - `Status` - Shows the Current (unless reset) kills, deaths, Quests [Accepted/Compeleted], pickups, relogins, and Current session time.

    - `Console` - **you will not use this unless told by a developer or know how to use it**

5. `Skills`
    - this is where you can edit our *PreMade* `SKillsets` to your liking (skill #'s shifted -1 what they are in-game so: 1-2-3-4-5-6 will become 0-1-2-3-4-5)

6. `Packets`
    - **you will not use this unless told by a developer or know how to use it**

7. `Bank`
    - Will Load your bank, this can also be brought up by pressing `B` (check hotkeys)

8. `Logs` - (preferably the `Scripts` tab)
    - Usually what the devs will ask for if you're having script issues (before crying to a script dev, please do these:
      1. Relog & restart the script
      2. in the 3rd tab in the `Skua Manager` press `Updates`, then `Scripts`, then `Reset Scripts`
      3. If none of this worked you may now come to us with your issue and provide screenshots and a good explanation of the issue.

9. `Plugins`
    - this will be Empty as of Current (unless you have the `Cosmetics Plugin [Beta]` by Lord Exelot [CosmeticsPlugin](<https://drive.google.com/file/d/1scL9o5bgaQLNZe-dRwrZbS-LOKx4jKeR/view?usp=share_link>))

10. `Auto`
    - **Auto-Attack** will automatically attack any of the monsters in the current cell.
    - **Auto-Hunt** will automatically hunt the selected monster across the entire map.
        - ***How to use:*** Click the monster in-game to select it. Then start Auto-Hunt. The bot will jump across the map to kill all instances of your selected monster. If no monster is selected before starting Auto-Hunt, it will consider all monsters in your current cell as selected and will hunt all of those instead.
    - **Class use mode** Press the refresh icon to reload the list of your class and skill mode to select. Then click "use select", and then hit either button to start. The Auto-Attack or Auto-Hunt will then use that skillset for its abilities.

11. `Jump`
    - Start by hitting `⟳` to get all cells and pads in the map.
    - **Current** gets the current cell and pad you're in.
    - **Jump** Jumps to selected cell and pad.

### How to install the Cosmetics Plugin for Skua

1. Download the .dll file from the link in the pins.
2. Locate the file on your PC (usually in your `/Downloads/` folder).
3. Copy (`ctrl+c`) or Cut (`ctrl+x`) the .dll file.
4. Go to the Skua plugin directory in your Documents (example: `C:\Users\YourName\Documents\Skua\plugins\`)
5. Paste (`ctrl+v`) the file.
6. Restart any Skua instances so you can see the plugin.
7. Within the Skua window, go to plugins (last option from the left) and then Cosmetics Plugin.
8. Repeat step 7 every time you wish to access the plugin.


### ***For further information or questions, Join the [Discord](https://discord.gg/pearlharbor)***
