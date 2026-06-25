# Autonomous Configuration Execution and Management Interface

# Introduction

AutoCEMI is a game launcher for common source ports of classic Doom. To start up the game, it needs a game profile in JSON format. Profiles can be written manually or created with the GUI application. Map and skill level are defined separately from game profile in execution options. IWAD and source port must be specified in the profile to launch the game.

Game profiles specify source port, IWAD, load order of mods, and additional arguments passed to the engine. They also contain name and description fields. The usage of game profiles is useful when you run multiple mod loads for different games and source port versions. They can be stored, written, and read with AutoCEMI anywhere in the file system given necessary permissions

Framework-dependent builds require specific versions of Windows App SDK and .NET runtime. The Inno Setup installer will download and install them automatically. If you do not want to install those dependencies, you can use self-contained builds instead. That option is not recommended for most users, as the size of self-contained applications is larger by about one hundred megabytes. I did manage to get the CLI application under 30 megabytes by trimming, but it is still noticeably bigger than framework-dependent build of the CLI.

AutoCEMI features include:

- Command line builds for PowerShell or batch scripting and faster access by terminal.
- Personal database of source ports, IWADs, and mods.
- Playtime statistics tracking. It is done by subtracting time at the moment when game profile execution is called and date time when process started by it exits.
- Map lump names extraction.

Version 1.0.0.0 has been tried with UZDoom (4.14.3) source port.

# Installation

## Option 1. (Recommended)

Prerequisites: administrator privileges, internet connection on target machine.

Download "autocemi-installer.exe" from the Releases page of tarskyie/AutoCEMI repo on GitHub dot com. Run the executable and follow instructions in the installer. It will need an internet connection to download dependencies. Click "ignore" to install dependencies later manually. AutoCEMI.CLI will be extracted to the same folder as GUI.

## Option 2

Download from the Releases page of tarskyie/AutoCEMI repo on GitHub dot com and manually extract zip folders "gui-win-x64-framework-dependent.zip" and/or "cli-win-x64-framework-dependent.zip". Install .NET Desktop runtime 10 and Windows App Runtime version 2.2.0.

## Option 3

Download from the Releases page of tarskyie/AutoCEMI repo on GitHub dot com and manually extract archive folders "gui-win-x64-self-contained.7z" and/or "cli-win-x64-self-contained.zip".

## How to uninstall AutoCEMI?

If you used the installer, run "unins000.exe" located in the program's root directory. Otherwise, delete the folder program was extracted to. You can also remove AutoCEMI directory from "%AppData%" that contains your user profile information (stats) and application's database.

# Usage

## Command-line interface application

- **Locate the executable**  
   Find AutoCEMI.CLI.exe and note its full path.  
   _Tip:_ If you use PowerShell, you can create a convenient alias in your \$PROFILE:

Set-Alias -Name autocemi -Value "C:\\Full\\Path\\To\\AutoCEMI.CLI.exe"

Then reload your profile with . \$PROFILE.

- **View available commands and options**  
   Run the program with the help flag:

```AutoCEMI.CLI.exe -help```

(or ```-h```, ```-?```)

**What AutoCEMI can do:**

- Launch games using saved profiles
- Add / remove IWADs, source ports, and mods to the database
- List all registered IWADs, source ports, and mods
- Display playtime statistics
- Reset application data

**Important notes:**

- The -force flag suppresses all confirmation prompts during execution.
- To launch a game, AutoCEMI requires the **IWAD**, **source port**, and all **mods** listed in the profile's load order to be present in the database.

**Creating your first game profile**

If you don't have a profile yet, create a new file with a .json extension (e.g. MyProfile.json) and paste the following template:
```JSON
{  
    "profile_version": "1.0",  
    "name": "",  
    "description": "",  
    "created": "",  
    "last_modified": "",  
    "game": {  
    "iwad": "",  
    "source_port": "",  
    "source_port_path": "",  
    "compatibility_level": "",  
    "custom_arguments": ""  
    },  
        "mods": {  
        "load_order": [],  
        "auto_sort": true  
    }  
}
```
**Quick setup:**

- Fill the iwad and source_port fields with names that already exist in the database (use the list command to check).
- Add mod filenames to the load_order array if needed.
- Save the file and run it with: AutoCEMI.CLI.exe -config "C:\\Path\\To\\MyProfile.json"

You can also use the add command to register IWADs, source ports, and mods into the database before launching.

## Graphical user interface application

### Interface and file IO overview

**Overview**

AutoCEMI GUI is a multi-tabbed game launcher for Windows 11 that lets you create, edit, and run game profiles, manage entries in the app's database, and view your play metrics.

Main page of the app features a tab viewer and a menu bar. You can add new tabs from the menu or by clicking the plus sign button on the top row after the open tab headings, and close them by clicking the cross sign next to the heading of the tab you want to close. Click on menu items to open expanders. Some options there are annotated with corresponding hotkeys.

**File opening and saving**

To open a saved game profile, select "Open" from the "File" menu. A file open dialog will open, select the item and click "Open". The tab will switch to the opened profile automatically.

To save a game profile, press Ctrl+S or select "Save" option from the "File" menu. If you want to write it into another file, press Ctrl+Shift+S or select "Save as".

**Keyboard accessibility**

To focus on the menu bar, press Alt. You can navigate the menu using access keys. Application will display access keys next to items they correspond to.

Keyboard shortcuts also allow you to interact with AutoCEMI without a mouse, but they don't require the user to navigate its menu.

| Hotkey combination | Action                               |
| ------------------ | ------------------------------------ |
| Ctrl+N             | Open new tab                         |
| Ctrl+O             | Open profile from a file             |
| Ctrl+S             | Save profile from currently open tab |
| Ctrl+Shift+S       | Save to another file                 |
| Ctrl+Q             | Exit application                     |
| F5                 | Execute profile                      |
| Ctrl+W             | Close tab                            |
| Ctrl+Tab           | Switch to the next tab               |

### Interacting with the application data

Select the "View and edit data" option in the "Execution" menu. Window will switch from main page to AutoCEMI data page. You can see your play statistics under "User Profile" section.

To add an IWAD / source port / mod click on the "Add" button under section. In opened dialog enter the name of item (it must be the same as in game profile) and path to file either manually or with a file picker. Path string must not contain quotation marks.

To remove an IWAD / source port / mod click on the cross icon button in the same row as its name.

This page interacts directly with the database through data service of the application, so the state of it is saved after every modification.

### Game profile execution

Open the profile you want to run and press F5 or choose "Run" from the "Execution" menu. To load a specific map from the IWAD, select "Edit Execution Parameters" in the same menu. Skill levels range from -1, where no difficulty argument is passed to the source port, up to 4, the maximum difficulty.

# Known issues

- Non-present and broken scroll-views in tab content and data editing page.
- Playtime contains and displays milliseconds, despite playtime tracking being only accurate by ±1 second.
- Closing application by Ctrl+Q or through menu does not save state of open tabs. 