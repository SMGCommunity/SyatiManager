# SyatiManager
A batch script that allows you to install, manage and compile Syati modules with ease.

## How to: Install Syati
1. Place SyatiManager.bat and installable_modules.csv in a new folder.
2. Double-click the bat and type `Y` to start the install process. This may take a while.
3. SyatiManager may prompt you to download `git` and/or `curl` if those are not installed on your system.
   
Video Guide: [https://youtu.be/MVdSGv_X6J0](https://youtu.be/xgrktYASQ0E)

## How to: Manage Modules and Build
Once Syati is installed, this batch script will become a module manager and builder.

Simply open SyatiManager.bat and type `M`. You will now see a list of Installed/Enabled Modules, Disabled Modules and Installable Modules. Type any module ID to view its details and to enable/disable/install them respectively.

After you have set up your modules right, type `B` and then the first letter of the region to build for. You should afterwards find a `CustomCode_{YourRegion}.bin` in the Output folder.

Video Guide: [https://youtu.be/EejOWQLaP3Q](https://youtu.be/qDcOk4mtMDw)

## How to: Add new modules
Currently the process of adding a new module is to simply add a new line to the `installable_modules.csv` on GitHub. SyatiManager will automatically download it every time you open it, to stay up to date.

A correct module entry should have the following:
1. The name of your new module
2. Your name/The name of the author
3. A basic description of what the module does
4. A download method. You can currently pick between `git`, `git_r` (recursive) or `curl` (which should lead to an archive file that 7zip can extract).
5. A download link.

Example of such an entry:
```csv
My cool Module;YourName;This modules is awesome, check it out!;git;https://github.com/YourName/YourRepo
```
Dependency modules are also supported, just specify them in your ModuleData.json as normal.

## Future plans
This tool is meant to be temporary until a full-fledged web app exists. However, I do want to make the process a little easier and more polished, by adding JSON support instead of basic csv. Also, Linux support is in the works.
