import os
import sys
import json
from urllib import request

sortInstallableJson = True
moduleInfos = list()
argc = 0
for argument in sys.argv:
    if argument == "--no-sort":
        sortInstallableJson = False
    elif (argc): # Not first argument
        moduleInfos.append(argument)
    argc += 1

if (argc == 1):
    print("Usage:\nAddToInstallables.py <PathToModuleInfo> [PathToModuleInfo] [...] [--no-sort]")
    exit(1)
if (not moduleInfos):
    print("Error: No module info was specified.")
    exit(1)

print("Getting newest database...")
try:
    with request.urlopen("https://github.com/SMGCommunity/SyatiManager/raw/main/installable_modules.json") as req:
        installableJson = req.read()
    installableModules = json.loads(installableJson)
except:
    print("Error: Could not get newest database.\nAbort.")
    exit(1)

print("Adding to installable modules...")
with open("installable_modules.json", "r+") as installableModules:
    installableModuleData = json.load(installableModules)
    for moduleInfo in moduleInfos:
        with open(moduleInfo, "r") as f:
            moduleData = json.load(f)
        if ("InstallType" not in moduleData):
            match (input("Install Type? ([l] = lginc_module, [g] = git, [gr] = git_recursive, [uz] = url_zip, [ut] = url_tar) ").lower()):
                case "l":
                    moduleData["InstallType"] = "lginc_module"
                case "g":
                    moduleData["InstallType"] = "git"
                case "gr":
                    moduleData["InstallType"] = "git_recursive"
                case "uz":
                    moduleData["InstallType"] = "url_zip"
                case "ut":
                    moduleData["InstallType"] = "url_tar"
        if ("InstallUrl" not in moduleData):
            moduleData["InstallUrl"] = input("Install URL? ")
        folderName = os.path.splitext(os.path.basename(moduleData["InstallUrl"]))[0]
        installableModuleData.append({"Name": moduleData["Name"], "Author": moduleData["Author"], "Description": moduleData["Description"], "InstallType": moduleData["InstallType"], "InstallUrl": moduleData["InstallUrl"], "FolderName": folderName})
print("Repacking installable_modules.json...")
json.dump(installableModuleData, open("installable_modules.json", "w"), indent = 4, sort_keys = sortInstallableJson)
print("Done!")
exit(0)