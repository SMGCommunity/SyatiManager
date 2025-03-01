<p align="center">
    <img src="SyatiManager/Assets/Logo.png" width="600">
    <p align="center">A C# application for managing <b>Syati Modules</b>, now with a GUI.</p>
</p>

## Features
- Creating and managing **Syati Solutions**.
- Downloading Modules through the **Module Library**.
- Setting up modules with Presets through the **Preset Library**.
- Compiling **Custom Code** and **Loaders**, along with generating a **Riivolution XML**.
- Various **QOL features**, such as generating **C++ properties** for **Visual Studio Code**.

## Authors
- [**VTXG**](https://github.com/VTXG): Main programmer/GUI designer.
- [**Bavario**](https://github.com/bavario-lginc): Programmer/Original SM developer.
- [**Lord-Giganticus**](https://github.com/Lord-Giganticus): Programmer.
- [**SY24**](https://github.com/SY-24): Logo and icon designer.
- [**LGINC Members**](https://github.com/Lord-G-INC): Testing and suggestions.

## Adding modules to the Module Library
Adding modules to the Module Library can be done by editing the `Modules.json` file inside `SyatiManager/Components/`. Entries should follow this format:
```json
{
  "Name": "...",
  "FolderName": "...",
  "Description": "...",
  "Author": "...",
  "Categories": [],
  "Install": "..."
},
```
`Name`: Your module's display name (e.g.: "Syati Initializer Module")<br>
`FolderName`: Your module's folder name, should not contain spaces (e.g.: "Syati_Init")<br>
`Author`: The author(s) of your module.<br>
`Description`: Your module's description. If you feel like the description is too big, you can enter `[External]` as your module's description and it will get a readme based on `Install`'s Url.<br>
`Categories`: Your module's categories. Try to use the same categories as other modules. If the existing categories do not fit your module, feel free to create a new one.<br>
`Install`: Where to download the module from. Check the [Install Source System](https://github.com/Lord-G-INC/Syati-Manager?tab=readme-ov-file#install-source-system) section of this readme.

## Adding presets to the Preset Library
Adding presets to the Preset Library can be done by editing the `Presets.json` file inside `SyatiManager/Components/`. New entries should follow this format:
```json
{
  "Name": "...",
  "Description": "...",
  "Author": "...",
  "Install": "..."
},
```
`Name`: Your preset's display name (e.g.: "LiveActor Preset")<br>
`Description`: Your module's description.<br>
`Author`: The author(s) of your preset.<br>
`Install`: Where to download the preset from. Check the [Install Source System](https://github.com/Lord-G-INC/Syati-Manager?tab=readme-ov-file#install-source-system) section of this readme.

## Install Source System
This system is used in `Modules.json` and `Presets.json` to clone repos and repo folders.<br>
Install Sources follow the `Type:Url` syntax.

`Type` should be replaced with one of the following types:
- `git`: Clones a git repo
- `git_recursive`: Clones a git repo and its submodules.
- `git_folder`: Downloads a specific folder from a repo.
- `lginc`: Modular-PTD module. Works like `git_folder`, but only requires the folder name.

`Url` should be replaced with:
- A repository url. (For `git` or `git_recursive`)
- A repository folder url. (For `git_folder`)
- A Modular-PTD module folder name. (For `lginc`)

Examples of valid install sources:
- `git:https://github.com/SMGCommunity/Syati_Init`
- `git_folder:https://github.com/SuperHackio/SMG2_AudioTestObjects/tree/Master/SMG2_SoundTestObj`
- `lginc:BlueCoinSystem`