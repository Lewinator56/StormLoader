<h1 align="right">StormLoader User Guide</h1>

## Contents
1. Installation
2. Usage
    1. Getting Started
    2. Loading a Mod Package
    3. Activating or Deactivating a Mod
    4. Uninstalling a Mod
    5. Viewing Mod Details
    6. Using Profiles
    7. Changing Settings
    8. Using the Repository
3. Information For Modders
    1. The .SLP Format
    2. Using the Repository Manager
4. Bug Reports

## Installation
- Extract the supplied zip folder to a directory of your choice.
- If you wish, reight click 'StormLoader.exe' and create a shortcut to it.

## Usage
### Getting Started
- When you first run stormloader, you will be prompted to enter the game location and the mod extraction location.
![image](https://user-images.githubusercontent.com/56686419/130524195-e4c958ca-4526-4f7f-b859-bfabfa9b4c15.png)
![image](https://user-images.githubusercontent.com/56686419/130524251-e737551a-1622-494a-b7ce-25e7fb553712.png)<br>
*The path here should be the path to the root stormworks location, this is the folder that contains the stormworks executable.*<br><br>
![image](https://user-images.githubusercontent.com/56686419/130524478-e94d332d-05bb-4104-9a8d-bbf26f9414e4.png)<br>
*This is the path to the folder you wish to store extracted mods, I highly reccomend you leave this blank.*<br><br>
- After completing the setup, you will be presented with the main window.

### Loading a Mod Package
- Mod packages are distributed as the .slp format, this is designed to guarantee mods will load correctly.
- StormLoader can load .zip files, however, the capability of mods based on a .zip is limited.
- To add a mod, click the 'Add mod from file' button in the top left corner, and browse to the mod package you wish to install.
![image](https://user-images.githubusercontent.com/56686419/130524821-6e5d816f-1216-42b3-aa98-18d6dd7aa34f.png)
- You may also add a mod from online sources, StormLoader has its own repository, and will eventually have nexus mods integration. To do this, click the 'Browse online' button and select the appropriate choice from the dropdown.<br>
![image](https://user-images.githubusercontent.com/56686419/130524991-cbade7d0-c108-4201-85cc-8ed95f022cc3.png)
- The mod will install and will be displayed in the listing under the command bar. By default, the mod will be enabled, this is indicated by a green checkmark.<br>
![image](https://user-images.githubusercontent.com/56686419/130525177-592e6f94-ac56-4368-bd51-3d2de4299ea7.png)

### Activating or Deactivating a Mod
- To activate a mod, simply click the grey checkmark on the right of its list item.
- If the mod is already installed, you will be prompted to confirm if you wish to overwrite it.
- If the mod installs a part that is installed by another mod, you will be prompted to confirm if you wish to overwrite it.
- To deactivate a mod, click the grey cross on the right of its list item.
- Deactivating a mod will reactivate any files the mod has overwritten installed by other mods, if, and only if, the mod is at the top of the list of overwrites for the specific files.
- *Mod installs are queued, for big mods, or for lots of installs at once, you can see the progress by looking at the install progress box at the bottom of the window*
![image](https://user-images.githubusercontent.com/56686419/130525409-0fd08ae3-81bf-4f03-81b6-e20cde45b16f.png)

### Uninstalling a Mod
- DO NOT MANUALLY DELETE MODS FROM THE STORMWORKS DIRECTORY!
- A mod can be uninstalled by clicking the grey 'bin' (or trash) icon to the right of its list item. This will completely remove all traces of the mod from your system.

### Viewing Mod Details
- On the right hand side of the main window is the information pane. This displays metadata about the mod, it also offers support for embedded html files, should the mod creator wish to include this with their mod.
