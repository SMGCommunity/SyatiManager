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
> Content-wise, an `.syt` file is just a `.json` file.

A default custom code project is structured like this:
```
├── Modules/
├── Output/
└── Solution.syt
```

## Module Library
A database of publicly available modules. They are stored in `SyatiManager/Components/Modules.json` and are automatically updated whenever the app starts.<br>
Adding new entries guide can be found [here](https://github.com/SMGCommunity/SyatiManager/blob/main/Docs/Guides/AddingToLibs.md#adding-modules-to-the-module-library).

## Preset Library
A database of publicly available presets. They are stored in `SyatiManager/Components/Presets.json` and are automatically updated whenever the app starts.<br>
Adding new entries guide can be found [here](https://github.com/SMGCommunity/SyatiManager/blob/main/Docs/Guides/AddingToLibs.md#adding-presets-to-the-preset-library).

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