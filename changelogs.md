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

