# SyatiManager
Python scripts that allows you to install, manage and compile Syati modules with ease.

## How to: Install Syati
1. Download SyatiSetup.py and SyatiManager.py from this repo, or just the whole repo.
2. Run `python SyatiSetup.py` in a command prompt, type `Y` and press enter to install.
3. SyatiManager may prompt you to download `git` and/or `curl` if those are not installed on your system.

## How to: Manage Modules and Build
Once Syati is installed, you can run `python SyatiManager.py` to manage Syati Modules and build them.

Simply run SyatiManager.py type `M` and press Enter. You will now see a list of Installed/Enabled Modules, Disabled Modules and Available Modules. Type any module ID to view its details and to enable/disable/install them respectively, or multiple module IDs seperated by commas to automatically enable/install them.

After you have set up your modules right, type `B` and press Enter. Afterwards type the first letter of the region to build for, or simply press Enter to build all regions. You should afterwards find a `CustomCode_{YourRegion}.bin` in the Output folder.

If you do not have a Syati Loader yet, type `L` and press Enter. Type `F` if you do not have a Riivolution XML yet, or `P` if you have one and simply want to add the relevant patches. Afterwards type the first letter of the region to build for, or simply press Enter to build all regions. You should afterwards find a `riivo_{YourRegion}.xml` in the Output folder.

If you want to make a new module from scratch, type `N` and press Enter. Then type in the Name, Author and Description. SyatiManager will then create a new blank module for you to use.

## How to: Add new modules to the database
Currently the process of adding a new module is to simply add a new entry to the `installable_modules.json` on GitHub. SyatiManager will automatically download it every time you open it, to stay up to date.

A correct module entry should have the following:
1. The name of your new module
2. Your name/The name of the author
3. A basic description of what the module does
4. A download method. You can currently pick between `git`, `git_recursive`, `git_folder`, `url_tar`, `url_zip`.
5. A download link.
6. The folder name of your Module.

Example of such an entry:
```json
{"Name": "My cool Module", "Author": "Bavario", "Description": "This module is amazing!", "InstallType": "git", "InstallUrl": "https://github.com/bavario-lginc/MyCoolModule", "FolderName": "MyCoolModule"}
```

If you use `git_folder`, instead of providing an `InstallUrl`, you need to instead provide a `GitRepo` and `GitPath`:
```json
{"Name": "My cool Module", "Author": "Bavario", "Description": "This module is amazing!", "InstallType": "git_folder", "GitRepo": "bavario-lginc/MyCoolModule", "GitPath": "MyCoolFolder/MyCoolSubfolder", "FolderName": "MyCoolSubfolder"}
```

## Additional support
### Build Tasks
By specifying a build task in the ModuleInfo.json file of your Module, SyatiManager will run the task in the command prompt. The only exception is `SyatiManager_copydisc`, which copies the contents of the Module's disc/ folder into the collective Output/disc/ folder.
```json
{
    "BuildTasks": ["SyatiManager_copydisc", "echo hello"]
}
```

### Install Dependencies
By specifying an install dependency in the ModuleInfo.json file of your Module, SyatiManager will check if the desired dependency is installed and enabled. If not, it will simply install/enable that dependency.
```json
{
    "InstallDependencies": ["Syati_CommonFunctionHooks"]
}
```

### ObjectDB Generation
By specifying an objectdatabase entry in the ModuleInfo.json file of your Module, SyatiManager will add the entry to the objectdb.json file of the user. This allows them to have the object documented when using it in other tools, like Whitehole.
```json
{
    "ObjectDatabaseEntries": [{
        "InternalName": "MyCoolObject", "Name": "My Cool Object", "Notes": "A very cool object that does amazing things!", "Games": 2, "Progress": 2
    }]
}
```