# Terminology
This page will teach you about concepts and terminology found all throughout SyatiManager.

## Modules
Modules are individual custom code packages for SMG2. Their files are usually layed out like this:
```
├── disc/
├── include/
├── source/
├── ModuleInfo.json
└── README.md
```
`disc`: An optional folder that contains archives for the game.<br>
`include`: A folder that contains header files for the module.<br>
`source`: A folder that contains C++ code for the module.<br>
`ModuleInfo.json`: Stores information about the module, such as the name, authors, codegen information, ect.<br>
`README.md`: Optional (but recommended) markdown file with documentation of the module.

## Codegen
A system used in Modules for code generation. Some notable examples of codegen usage include `Syati_ObjectFactories` and `Syati_CommonFunctionHooks`.

## Syati Solutions
Syati Solutions (`.syt`) are project files for custom code. They store your project's module folder, output folder, unibuild configuration, build regions and build tasks.

> [!NOTE]
> Content-wise, a `.syt` is just a `.json`.

A default custom code project is structured like this:
```
├── Modules/
├── Output/
└── Solution.syt
```

## Module Library
A database of publicly available modules. They are stored in `SyatiManager/Components/Modules.json` and are automatically updated whenever the app starts.
Entries should follow this format:
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
`Description`: Your module's description.<br>
`Categories`: Your module's categories. Try to use the same categories as other modules. If the existing categories do not fit your module, feel free to create a new one.<br>
`Install`: Where to download the module from. Check the Install Source section.

## Preset Library
A database of publicly available presets. They are stored in `SyatiManager/Components/Presets.json` and are automatically updated whenever the app starts.
New entries should follow this format:
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
`Install`: Where to download the preset from. Check the Install Source section.

## Install Source
This system is used in `Modules.json` and `Presets.json` to clone repos and repo folders.<br>
Install Sources follow this syntax: `type:url`.

`type` should be replaced with one of the following types:
- `git`: Clones a git repo
- `git_recursive`: Clones a git repo and its submodules.
- `git_folder`: Downloads a specific folder from a repo.
- `lginc`: Modular-PTD module. Works like `git_folder`, but only requires the folder name.

`url` should be replaced with:
- A repository url. (For `git` or `git_recursive`)
- A repository folder url. (For `git_folder`)
- A Modular-PTD module folder name. (For `lginc`)