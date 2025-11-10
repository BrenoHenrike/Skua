# Skua 1.3.0.0
## Released: November 10, 2025

### We've moved to a [new git](https://github.com/auqw/Skua/)

---

# Skua 1.2.5.4
## Released: September 12, 2025

## HOTFIX FOR GET SCRIPTS



**Full Changelog**: https://github.com/BrenoHenrike/Skua/compare/1.2.5.3...1.2.5.4

---

# Skua 1.2.5.3
## Released: September 12, 2025

## HOTFIX FOR INSTALLER


**Full Changelog**: https://github.com/BrenoHenrike/Skua/compare/1.2.5.2...1.2.5.3

---

# Skua 1.2.5.2
## Released: September 12, 2025

## HOTFIX FOR ARCHMAGE

**Full Changelog**: https://github.com/BrenoHenrike/Skua/compare/1.2.5.1...1.2.5.2

---

# Skua 1.2.5.1
## Released: September 03, 2025

## What's Changed

## NET 9 WAS BAD FOR SKUA SO I REVERTED BACK TO NET 6 (also I actually didn't even change to net 9 for all projects that was my bad)
### Health Check
- anytime we pull from ScriptMonsters we pulled from MapMonsters which didn't have the dataleaf connected to it
- added MapMonstersWithCurrentData which pulls the dataleaf from the monsters then merges it

### Fixed Keybinds
- I broke keybinds while fixing mem leaks (*still broken D:* )
- Keybinds should only work when focus into the game now

## **Full Changelog**: https://github.com/BrenoHenrike/Skua/compare/1.2.5.0...1.2.5.1

---

# Skua 1.2.5
## Released: September 02, 2025

## What's Changed

### 1. FlashUtil.cs - Flash Object Disposal Issues

**Problem**: The Flash object's event handlers were not unsubscribed before disposal, and the object reference wasn't cleared.

**Fix**:

- Unsubscribe `FlashCall` event before disposing Flash object in `InitializeFlash()`
- Properly clean up in `Dispose()` method
- Set Flash reference to null after disposal

### 2. ScriptInterface.cs - Thread and Event Handler Leaks

**Problem**:

- FlashCall event never unsubscribed
- ScriptInterfaceThread not properly terminated
- CancellationTokenSources not disposed
- Static instance reference not cleared

**Fix**:

- Implemented full IDisposable pattern
- Proper thread termination with timeout
- CancellationTokenSource cleanup
- Event handler unsubscription
- Static reference clearing

### 3. ListBoxScrollToCaretBehavior.cs - WPF Behavior Memory Leak

**Problem**: CollectionChanged event subscriptions weren't properly managed, causing collections to hold references to behaviors.

**Fix**:

- Store reference to collection source
- Properly unsubscribe using stored reference
- Clear ScrollViewer reference on unload

### 4. ScriptEvent.cs - Messenger Registration Leak

**Problem**: Registered with StrongReferenceMessenger but never unregistered, keeping object alive indefinitely.

**Fix**:

- Implemented IDisposable
- Unregister all messenger subscriptions in Dispose()
- Clear all event handlers
- Added proper cleanup for all events

### 5. LogService.cs - TraceListener Memory Leak

**Problem**: TraceListener added to global Trace.Listeners but never removed, creating permanent reference.

**Fix**:

- Store reference to DebugListener
- Implemented IDisposable
- Remove listener from Trace.Listeners in Dispose()
- Clear log collections
- Unregister from messenger

### 6. CaptureProxy.cs - TcpClient Resource Leaks

**Problem**: TcpClient instances created in loop without proper disposal on exceptions.

**Fix**:

- Use local variables for proper scoping
- Ensure cleanup in catch blocks
- Add finally blocks for connection cleanup
- Properly handle cancellation

### 7. ScriptManager.cs - Thread Management Issues

**Problem**: Thread references overwritten without cleanup, no disposal pattern.

**Fix**: (hopefully didn't break anything)

- Thread cleanup should be added with proper join/wait
- IDisposable pattern should be implemented
- CancellationTokenSource proper disposal

### 8. SavedAdvancedSkillsViewModel.cs - ObservableCollection Recreation

**Problem**: Creates new ObservableCollection on every property access, causing memory churn and binding issues.

**Fix**:

- Cache ObservableCollection instance
- Only recreate when data actually changes
- Proper null checking and refresh logic

### 9. @!HotKeyService.cs - InputBindings Memory Leak

**Problem**: InputBindings added to MainWindow without proper cleanup, accumulating over time.

**Fix**:

- Implemented IDisposable
- Track registered bindings in a list
- Clear bindings before adding new ones
- Proper cleanup in Dispose()

### 10. DialogService.cs - Event Handler Exception Safety

**Problem**: Event handler might not be removed if ShowDialog throws exception.

**Fix**:

- Added try-catch blocks around ShowDialog
- Ensure event handler removal in all cases
- Handler removal in catch block

### 11. AutoViewModel.cs - Task.Factory Without Cancellation

**Problem**: Tasks started without CancellationToken can't be properly cancelled.

**Fix**:

- Implemented IDisposable
- Added CancellationTokenSource management
- Proper task cancellation and disposal
- Unregister from messenger in Dispose()

### 12. AdvancedSkillContainer.cs - Fire-and-Forget Tasks

**Problem**: Fire-and-forget tasks without error handling or cancellation.

**Fix**:

- Implemented IDisposable
- Added CancellationTokenSource for save operations
- Track save tasks for proper cleanup
- Error handling in save operations

### 13. WindowService.cs - Managed Windows Dictionary

**Problem**: Windows added to dictionary but never removed when closed.

**Fix**:

- Implemented IDisposable
- Add Closed event handler to remove from dictionary
- Clean up DataContext if IDisposable
- Dispose all managed windows on service disposal

### 14. PropertyGrid.xaml.cs - PropertyChanged Weak Event

**Problem**: PropertyChanged event subscription without weak references.

**Fix**:

- Use WeakEventManager for PropertyChanged subscriptions
- Try-catch for safe unsubscription
- Prevents strong references from keeping objects alive

# **Full Changelog**: https://github.com/BrenoHenrike/Skua/compare/1.2.4.0...1.2.5.0

## If you want to help the development of Skua you can donate by clicking on the button below:

### purple/SharpTheNightmare: [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/paypalme/sharpiiee?country.x=US&locale.x=en_US)

### Breno Henrike's PayPal [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

### Lord Exelot's PayPal [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/LordExelot)

---

# Skua 1.2.4
## Released: April 05, 2024

## What's Changed
* New discord links by @LordExelot in https://github.com/BrenoHenrike/Skua/pull/46
* Auto attack fix by @SharpTheNightmare in https://github.com/BrenoHenrike/Skua/pull/52
* Reverted this commit https://github.com/BrenoHenrike/Skua/commit/37511a5aac9986610dc43f9cf8c8bb5327e8ed3c as it caused too many issues
* Changed the Kill/Hunt For Item method so it kills multiple monsters that may exist in the same room. https://github.com/BrenoHenrike/Skua/commit/7c919fed4ffe3e553111378fde194a3c66573ff5 and https://github.com/BrenoHenrike/Skua/commit/3b4d164f2096c3e11e660801115356fe9dfada5c
* Changed grabber according to changes in Shops.BuyItem. Also got rid of equip freeze. https://github.com/BrenoHenrike/Skua/commit/b261e0382400e9c8edb6fb68b732c022adb22ef0
* Added quantity parameter to BuyItem and renamed methods that loaded the shop first. https://github.com/BrenoHenrike/Skua/commit/fa8b7896a9710e1af75dcedd652b860e4aab4261
* Added back Load Game request so it works when launching from the exe. https://github.com/BrenoHenrike/Skua/commit/249201dc7afd5d99ca76728f20353eb226e2021f
* Make Exists method check for MapID too. https://github.com/BrenoHenrike/Skua/commit/e88d7b350abf2b989d396565ffd64264b1901718
* Added Sound Service, for now, it only beeps. https://github.com/BrenoHenrike/Skua/commit/1afcc96bc02055d9a6ceb04ef009f96e52e6c5a6
* Added Notify Drop Window. (Not enabled) https://github.com/BrenoHenrike/Skua/commit/59af65e114130cd3323b047ffc6383be6083da8d


**Full Changelog**: https://github.com/BrenoHenrike/Skua/compare/1.2.3.0...1.2.4.0



## New Contributors
* @SharpTheNightmare made their first contribution in https://github.com/BrenoHenrike/Skua/pull/52


If you want to help the development of Skua you can donate by clicking on the button below:

### [Skua Creator] Breno Henrike 
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

---

# Skua 1.2.3
## Released: March 17, 2023

## Fixes
- Hunt methods work as intended again²;
- Account Manager Login Server has a default value again;
- Small delay between each launch to prevent file access exceptions;
- Fixed ScriptAuto possible bug when stopping.

If you want to help the development of Skua you can donate clicking in the button below:

[Skua Creator] Breno Henrike
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY).

[Developer] Yor Delfina
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=DMZFDRYJ5BT96)

---

# Skua 1.2.2
## Released: March 14, 2023

## Fixes
- Hunt methods work as intended again;
- Server objects have their JsonConverters again;
- CustomWindow now respects the Min* and Max* properties;

## Additions
- Account Manager accounts list has the option to show more/less columns;
- Account Manager label to show how many accounts are selected;
- Account Manager now allows to launch accounts and start the specified script;
- Settings from Skua and Skua Manager now persist between versions. No more need to add accounts again;
- ThemeService now has 2 events to help changing themes in plugins.

If you want to help the development of Skua you can donate clicking in the button below:

[Skua Creator] Breno Henrike
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY).

[Developer] Yor Delfina
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=DMZFDRYJ5BT96)

---

# Skua 1.2.1
## Released: February 06, 2023

## What's new?
- **Change logs**; you can now see the changelogs from the Skua Manager.
   - Change logs will pop up after auto update and restart Skua Manager.

## Major fixes:
- **Get Scripts Data** null values was replaced to "No description provided." and "no-tags" for tags in the client side.
- **Shop Loader in Loader tab**; now you can load shop from the loader tab without freezing your client.

---

# Skua 1.2
## Released: February 04, 2023

# Major Fixes:
- Default client files location was changed to Users/Documents directory.
- Default client update download path was changed and triggered to Users/Downloads directory.
- Skua Installer: App shortcuts, Directory path naming, and the version upgrader.
- New User Interface for get scripts:
   - script infos: name, description, scriptpath, and tags
   - tags as badge.
- Referring above, we migrated it to the json as data source that was automatically generated by the github actions just to remove the github api limit issues.
   - Client updates (data source)
   - Get Scripts (data source)
- Added some properties in Client settings:
   - Auto update advanced skill sets;
   - Check advanced skill sets updates;
- Modified some name of the properties in Client Settings:
   - Auto update scripts;
   - Check scripts updates;
- Fixed Client released updates for pre-releases.
- Fixed Advanced Skill Sets bugs.
- Fixed Skua plugin feature.
- Fixed Account Manager Bugs
- Fixed About Page crashes
- For client updates I included `msi` execution, and the zip extraction is still there and not changed.
- Modified Jump method with auto correction map padding.
- Optimized auto attack, and auto hunt function.
- Average optimized functions of clients by removed redundancies.
- Remove Github Authentication temporarily.

# What's new?
- **Skua Manager Account Manager**; successfully implemented by @BrenoHenrike
- **About Skua** detailed content about skua and etc. 
- **Skua Installer & Auto Updates**; As based on the survey we successfully fixed the Skua Installer and also for auto update client on download new version. **Note that we are using installer anymore so maybe the old versions might give crashes because we will no longer package the product into zip file**. 


[Artist of Skua] Breno Henrike
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY).

[Developer] Yor Delfina
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=DMZFDRYJ5BT96)

---

# Skua 1.1.4
## Released: January 07, 2023

### What's new?
- Aura reading to be used in bot scripts like (CoreArmy: to read player auras based on the skills or consumables)
- Auto/Check update advance skill sets settings added in the `Options > Application`

### Fixed tools
- Grabber tool

### Existing feature (if you never notice this feature)
- You can toggle show/hide Skua Bot Client window in tray

### Client Changes
- Skua manager can be now closed (process will run in background)
   - No more multiple skua manager window on startup skua.manager.exe
   - Tray skua options:
        - Show Skua manager (if closed)
        - Skua Bot AQW (launch new skua bot client instance) 
        - Update Scripts
        - Reset Scripts
        - Check client options
        - Exit  (All Skua windows manager, & bot clients will be terminated)
        
If you want to help the developer to make skua better just click the buttons.
[Artist of Skua] Breno Henrike
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY).

[Developer] Yor Delfina
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=DMZFDRYJ5BT96)

---

# Skua 1.1.3.1
## Released: January 02, 2023

# Major Fixes:

### Fixed issues:
- Skua manager for client management 🛠️
     - On close client in the skua manager:
        - will now remove also in the skua manager cleint list;
        - all windows from a single client will be closed (the prev issue was the child window will only close);
     - Changes:
         - WARNING: On close skua manager all skua clients will be closed (be careful 💀);
- Advanced Skill Sets 🔥
     - Rebased AdvancedSkillSets data link for auto update on launch skua;
     - On save edited skill is now interactive no more refresh (but the refresh button is still in there for in a case)
     - On reset skill sets is also now interactive no more refresh.
 - Bot Script paths was already fixed by  **Lord Exelot#9674**

If you want to help the developer to make skua better just click the buttons.
[Artist of Skua] Breno Henrike
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY).

[Developer] Yor Delfina
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=DMZFDRYJ5BT96)

---

# Skua 1.1.3
## Released: December 31, 2022

# Major Fixes:

### Fixed issue:
- Path of options and scripts files 📂
- On save user defined skill sets are now dynamic 🛠

### What's new?
- Auto update skill sets but need to refresh in the skill tab to be able to update the lists skills in the user interface;
   (but behind data skill sets process is dynamic)
- Reset Skill Sets to default;

If you want to help the developer to make skua better click the buttons.
[Artist of Skua] Breno Henrike
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY).

[Developer] Yor Delfina
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=DMZFDRYJ5BT96)

---

# Skua 1.1.2.2
## Released: December 31, 2022


# Major fixes:
 ### Fixed large delays of the following:
   - On Auto attack/hunt ⚔️
   - On Stop auto attack/hunt 🛑
   - On Equip Item 🛠️
   - On Use skill set 🪄

## *File structure was changed based on the survey result:*
![Skua survey](https://img001.prntscr.com/file/img001/eMmSYj3QTQOOJQVrws-ZIg.png)

---

# Skua 1.1.2.1
## Released: December 28, 2022

This is hot fix for the current issue that has been made an error from game action on start script. 

Fixes: 
- Game action type changed from non nullable to nullable to make the crash will not happen again.


[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

---

# Skua 1.1.2
## Released: December 28, 2022

Quite some time I don't do this, the update fixes (I hope) some small but troublesome bugs:

- Hide to tray should not crash the app;
- "GitHub API limit reached" can still happen, but all you need to do is to authenticate one time and launch Skua using the launcher tab from the Manager, after that the auth token should be saved as it was supposed to;
- For some reason starting Auto Attack/Hunt froze the app, it will not happen anymore but it does take quite some time to start/stop, will keep that in mind for possible future updates;

That's not much I would say, there is some work done that I can't publish yet that lacks testing and proper implementation, with time they will come.

Also, @slypy been looking foward to help with Skua development, and before his interest in doing so, I never knew the pain in the ass it was to set up the source code to start working on it. So I uploaded the whole (even the WIP things I had going on) of it in the repo, hope that helps anyone that would like to contribute.

If you want to help the development of Skua you can donate clicking in the button below:

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

---

# Skua 1.1.1
## Released: September 08, 2022

Release to address some small problems:

- Enums that make use of "_"(underscore) to generate spaces in the UI should not crash the script compilation anymore;
- Fixed the size of script options. For some reason the cells where not taking the full width, making the fields too small to even see;
- Map Item ID grabber take into consideration swf files that might not have the ".swf" extension, this fix a conflict with how FFDec takes arguments;
- Options for _"Auto Update Scripts"_ and _"Check for Script Updates"_ back to the main client;
- `IScriptServer` Relogin methods now click the server instead of sending a command straight to connect to said server, this way the "Server" in the game options will show the right server name instead of the first server you logged in.

If you want to help the development of Skua you can donate clicking in the button below. To select any of the goal options (you can check then in the _"Goals"_ tab in the **Skua Manager**), put a message in the donation or message me in the Skua chat in [Discord](https://discord.io/AQWBots):

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

---

# Skua 1.1
## Released: September 03, 2022

Thsi release comes with the Skua.Manager.exe, with it you will be able to set up your GitHub Authentication a single time, that can be persisted between client versions. To do so, you complete the authentication in the Manager and then launch Skua using the "Launchers" tab, atleast one time. The Manager will also able to notify, download and configure future updates.

Fixes, additions and subtractions:

- Fixed bug with Auto Relogin when using Safe Relogin;
- Using the Interceptor doesn't trigger Auto Relogin anymore;
- Removed options from CoreBots > Other, they were added as Script Options;
- Added options in CoreBots > Other for Boost usage inside scripts;
- Added ability to hide to tray for both clients;
- Manager can Update and Reset (delete and redownload) Scripts in the Updates tab or right clicking the tray icon;
- Goals can be tracked in the Manager;
- Themes can be propagated from the Manager to other Launchers;

If you want to help the development of Skua you can donate clicking in the button below, to select any of the options, put a message in the donation or message me in the Skua chat in [Discord](https://discord.io/AQWBots):

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

---

# Skua 1.0.2
## Released: August 25, 2022

Did an oopsie in the last release. While trying to reduce server spam, it made the return value notc so certain, so the change to `IScriptQuest.EnsureLoad` was reverted.

About the goals:

25/200$ - Follower/Synchronize Clients so you can use multiple accounts;
0/100$ - Manager so you can boot up multiple accounts with the click of a button;
25/100$ - Script making assistant, something between Scratch (programming language) and formulary filling;
?$ - Give me any idea, we can discuss about it and make it real.

If you want to help the development of Skua you can donate clicking in the button below, to select any of the options, put a message in the donation or message me in the Skua chat in [Discord](https://discord.io/AQWBots):

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

---

# Skua 1.0.1
## Released: August 24, 2022

Fixes:

- Auto update would only update if there were scripts missing from the folder;
- `IScriptQuest.IsDailyComplete` method returns the proper value;
- Private Rooms option would be wrongly setted in CoreBots;
- ScriptStoppingEvent now is properly fired when the script crashes;
- `IScriptSend.PacketSpam` and `IScriptSend.ClientPacketSpam` now stop if you stop the script too;
- `IScriptServer.EnsureRelogin` uses the right method to try to login again;
- Scripts are shown when opening the "Get Scripts" window for the first time.

---

# Skua 1.0
## Released: August 20, 2022

The birb is out.

Everything should work as intended, next release should be mainly documentation if no bug shows up.

There are some out of scope goals, they aren't planned for the normal development of Skua, but reaching them not only would help me keep motivated but also the community that I am sure would benefit from those tools.

25/200$ - Follower/Synchronize Clients so you can use multiple accounts;
0/100$ - Manager so you can boot up multiple accounts with the click of a button;
25/100$ - Script making assistant, something between Scratch (programming language) and formulary filling;
?$ - Give me any idea, we can discuss about it and make it real.

If you want to help the development of Skua you can donate clicking in the button below, to select any of the options, put a message in the donation or message me in the Skua chat in [Discord](https://discord.io/AQWBots):

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

---

