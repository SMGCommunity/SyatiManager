import os
import sys
import subprocess
from urllib import request
from urllib.parse import urlparse
import json
import tarfile
import zipfile
import shutil
from pathlib import Path
class ModuleInfo:
    def __init__(self, items: dict[str] = {}):
        self.Name = str()
        self.Author = str()
        self.Description = str()
        self.InstallDependencies = list[str]()
        self.BuildTasks = list[str]()
        self.ObjectDatabaseEntries = list()
        self.ModuleType = ""
        self.FolderPath = ""
class InstallableModule:
    def __init__(self, items: dict[str] = {}):
        self.FolderName = str()
        self.Name = str()
        self.Author = str()
        self.Description = str()
        self.InstallType = str()
        self.InstallUrl = str()
        self.GitRepo = str()
        self.GitPath = str()
        self.ModuleType = ""
moduleData = list[ModuleInfo]()
installableModules = list[InstallableModule]()
useLocalData = False

def wingetInstall (packageName :str, displayName :str):
    if sys.platform == "win32":
        print(f"{displayName} was not found. Would you like to download it now?")
        prompt = input("[Y/N] ")
        if (prompt.lower() == "y" & subprocess.run(["winget", "install", f"{packageName}.{packageName}"]).returncode != 1):
            print(f"Error while installing {displayName}.")
            exit(1)
        else:
            exit(1)
    else:
        print(f"{displayName} was not found. Please download it using your standard package manager.")
        exit(1)

def copyFilesRecursive (srcPath :str, destPath :str):
    for member in os.listdir(srcPath):
        if (os.path.isdir(f"{srcPath}{member}")):
            if not os.path.isdir(f"{destPath}{member}/"):
                os.makedirs(f"{destPath}{member}/")
            copyFilesRecursive(f"{srcPath}{member}/", f"{destPath}{member}/")
        else:
            shutil.copy(f"{srcPath}{member}", f"{destPath}{member}")

def runBuildTask (buildTask :dict, srcPath :str):
    if (buildTask["Task"] == "Copy"):
        if not os.path.isdir(f"{srcPath}/{buildTask["To"]}"):
            os.makedirs(f"{srcPath}/{buildTask["To"]}")
        copyFilesRecursive(f"{srcPath}/{buildTask["From"]}", f"{srcPath}/{buildTask["To"]}")
    elif (buildTask["Task"] == "Command"):
        if (sys.platform == "win32" and subprocess.run(buildTask["win32"]).returncode):
            print("Error while running build task.")
            return False
        elif (subprocess.run(buildTask["linux"]).returncode):
            print("Error while running build task.")
            return False
    elif (buildTask["Task"] == "BuildLoader"):
        if (not buildLoaderScript(buildTask["Regions"], buildTask["FullLoader"], f"{srcPath}/{buildTask["OutputPath"]}")):
            return False
    else:
        print(f"Unknown task type {buildTask["Task"]}.")
        return False
    return True

def buildScript (regionList :list = list(), buildTasks :list = list(), outputPath :str = "Syati/Output", unibuild :bool = False):
    global moduleData
    if (not os.path.isdir(outputPath)):
        os.makedirs(outputPath)
    if (not regionList):
        print("Which region? Press Enter to build all regions.")
        match (input("[J] = JPN, [U] = USA, [P] = PAL, [K] = KOR, [T] = TWN: ").lower()):
            case "j":
                regionList = ["JPN"]
            case "u":
                regionList = ["USA"]
            case "p":
                regionList = ["PAL"]
            case "k":
                regionList = ["KOR"]
            case "t":
                regionList = ["TWN"]
            case _:
                regionList = ["JPN", "USA", "PAL", "KOR", "TWN"]
    for region in regionList:
        print(f"Building target {region}...")
        if (subprocess.run(["./Syati/SyatiModuleBuildTool", region, "Syati/Syati/", "Syati/Modules/", outputPath, "-u" if unibuild else ""]).returncode):
            print(f"Failed building target {region}. Abort.")
            return False
    objectdbPath = ""
    for module in moduleData:
        if (module.ModuleType != "enabled"):
            break
        print(f"Running build tasks for {module.Name}...")
        for buildTask in module.BuildTasks + buildTasks:
            if (not runBuildTask(buildTask, module.FolderPath)):
                return False
        for objectdbEntry in module.ObjectDatabaseEntries:
            if (not objectdbPath):
                objectdbPath = input("Please specify the path to Whitehole's objectdb.json: ")
            with open(objectdbPath, "r") as f:
                objectdbData = json.load(f)
            objectdbData.append(objectdbEntry)
            with open(objectdbPath, "w") as f:
                json.dump(objectdbData, f)
    print("\nBuilt successfully!")
    if os.path.isdir(f"{outputPath}/disc"):
        print("Note: Make sure to copy the disc/ folder into your game dump, otherwise the built modules may not work correctly.")
    return True

def buildLoaderScript (regionList :list = list(), makeFullXML :bool = None, outputPath :str = "../Output"):
    os.chdir("Syati/Syati")
    if (not os.path.isdir(outputPath)):
        os.makedirs(outputPath)
    if (makeFullXML == None):
        print("Full [F] or Partial [P] XML?")
        if (input("If you do not use a Riivolution XML yet, choose Full. ").lower() == "f"):
            makeFullXML = True
    if (not regionList):
        print("Which region? Press Enter to build all regions.")
        match (input("[J] = JPN, [U] = USA, [P] = PAL, [K] = KOR, [T] = TWN: ").lower()):
            case "j":
                regionList = ["JPN"]
            case "u":
                regionList = ["USA"]
            case "p":
                regionList = ["PAL"]
            case "k":
                regionList = ["KOR"]
            case "t":
                regionList = ["TWN"]
            case _:
                regionList = ["JPN", "USA", "PAL", "KOR", "TWN"]
    for region in regionList:
        if (subprocess.run(f"python buildloader.py {region} -o {outputPath} {"--full-xml" if makeFullXML else ""}").returncode):
            print("\nError while building the loader. Abort.")
            os.chdir("../..")
            return False
    os.chdir("../..")
    print("\nBuilt the loader successfully!")
    return True

def installModule (module :InstallableModule, installAllDeps :bool = False):
    global moduleData
    match (module.InstallType):
        case "git" | "git_recursive":
            if (subprocess.run(["git"], stdout=open(os.devnull, "wb")).returncode != 1):
                if (not wingetInstall("git", "Git")):
                    return False
            print(f"Cloning module {module.Name}...")
            if (subprocess.run(["git", "-C", "Syati/Modules", "clone", module.InstallUrl + (" --recursive" if module.InstallType == "git_recursive" else "")], stderr=open(os.devnull, "wb")).returncode):
                print(f"Cloning module {module.Name} failed. Abort.")
                return False
        case "lginc_module":
            print(f"Downloading module {module.Name}...")
            with request.urlopen(f"https://mariogalaxy.org/syati-modules?module={module.InstallUrl}") as req:
                moduleTar = req.read()
            with open("Syati/Modules/" + module.InstallUrl + ".tar.gz", "wb") as f:
                f.write(moduleTar)
            os.chdir("Syati/Modules/")
            with tarfile.open(module.InstallUrl + ".tar.gz") as f:
                f.extractall(".")
            os.chdir("../..")
            os.remove("Syati/Modules/" + module.InstallUrl + ".tar.gz")
        case "url_tar":
            print(f"Downloading module {module.Name}...")
            pathToModuleTar = "Syati/Modules/" + os.path.basename(module.InstallUrl)
            with request.urlopen(module.InstallUrl) as req:
                moduleTar = req.read()
            with open(pathToModuleTar, "wb") as f:
                f.write(moduleTar)
            with tarfile.open(pathToModuleTar) as f:
                f.extractall(".")
            os.remove(pathToModuleTar)
        case "url_zip":
            print(f"Downloading module {module.Name}...")
            pathToModuleZip = "Syati/Modules/" + os.path.basename(module.InstallUrl)
            with request.urlopen(module.InstallUrl) as req:
                moduleTar = req.read()
            with open(pathToModuleZip, "wb") as f:
                f.write(moduleTar)
            with zipfile.ZipFile(pathToModuleZip) as f:
                f.extractall(".")
            os.remove(pathToModuleZip)
    with open(f"Syati/Modules/{module.FolderName}/ModuleInfo.json", "r") as f:
        infoJson = json.load(f)
    moduleInfo = createModuleInfoFromJson(infoJson)
    moduleInfo.ModuleType = "enabled"
    moduleInfo.FolderPath = "Syati/Modules/" + module.FolderName
    moduleData.append(moduleInfo)
    if (not checkDependencies(moduleInfo, installAllDeps)):
        shutil.move(moduleInfo.FolderPath, "Syati/DisabledModules/")
        moduleInfo.FolderPath.replace("/Modules/", "/DisabledModules/")
        moduleInfo.ModuleType = "disabled"
        print("Abort.")
        return "disabled"
    print("Done.")
    return True

def getModuleFromFolderPath (moduleFolderPath :str):
    global moduleData
    for module in moduleData:
        if (module.ModuleType == "available"):
            continue
        if (module.FolderPath == moduleFolderPath):
            return module
    return False

def getInstallableModuleFromFolderName (moduleFolderName :str):
    global moduleData
    for module in moduleData:
        if (module.ModuleType != "available"):
            continue
        if (module.FolderName == moduleFolderName):
            return module
    return False

def checkDependencies (module :ModuleInfo, installAll :bool = False):
    for dependency in module.InstallDependencies:
        dependencyData = getModuleFromFolderPath("Syati/DisabledModules/" + dependency)
        if (dependencyData and not os.path.isdir("Syati/Modules/" + dependency)):
            if (not installAll):
                print(f"This module requires the disabled module {dependencyData.Name}. Enable it now?")
                if (input("[Y/N] ").lower() == "y"):
                    if (not checkDependencies(dependencyData)):
                        print("Abort.")
                        return False
                else:
                    print("Abort.")
                    return False
            shutil.move(dependencyData.FolderPath, "Syati/Modules/")
            dependencyData.FolderPath.replace("/DisabledModules/", "/Modules/")
            dependencyData.ModuleType = "enabled"
        elif (not os.path.isdir("Syati/Modules/" + dependency)):
            dependencyData = getModuleFromFolderPath("Syati/Modules/" + dependency)
            if (not dependencyData):
                dependencyData = getInstallableModuleFromFolderName(dependency)
                if (not dependencyData):
                    print(f"Fatal error: Module {dependency} is required but was not found.")
                    return False
                if (not installAll):
                    print(f"This module requires the installable module {dependency}. Download it now?")
                    if (input("[Y/N] ").lower() == "n"):
                        return False
                installModule(dependencyData)
    return True

def newModule ():
    os.system("cls" if sys.platform == "win32" else "clear")
    print("--- New Module ---")
    print("This will use the SyatiModuleTemplate to create a new blank module for you to use.\nPlease fill out the form and press Enter after every question.\n")
    newModuleData = dict()
    newModuleData["Name"] = input("Name: ")
    newModuleData["Author"] = input("Author(s): ")
    newModuleData["Description"] = input("Description: ")
    print("\nAre these details ok?")
    if (input("[Y/N] ").lower() == "n"):
        return False
    else:
        newFolderName = newModuleData["Name"].replace(" ", "")
        shutil.copytree("Syati/SyatiModuleTemplate", "Syati/Modules/" + newFolderName)
        subprocess.run(["rm", "-rf", f"Syati/Modules/{newFolderName}/.git"])
        os.remove(f"Syati/Modules/{newFolderName}/.gitattributes")
        os.remove(f"Syati/Modules/{newFolderName}/.gitignore")
        os.remove(f"Syati/Modules/{newFolderName}/include/.gitkeep")
        os.remove(f"Syati/Modules/{newFolderName}/source/.gitkeep")
        with open(f"Syati/Modules/{newFolderName}/ModuleInfo.json", "w") as f:
            json.dump(newModuleData, f)
    os.system("cls" if sys.platform == "win32" else "clear")

def showModuleDetails (moduleId :int):
    global moduleData
    os.system("cls" if sys.platform == "win32" else "clear")
    if (moduleData[moduleId].ModuleType == "enabled"):
        print("\033[32mEnabled\033[0m")
        print(f"Name: {moduleData[moduleId].Name}")
        print(f"Author: {moduleData[moduleId].Author}")
        print(f"Description: {moduleData[moduleId].Description}")
        match (input("[0] = Disable, [X] = Remove, [C] = Cancel: ").lower()):
            case "0":
                os.system("cls" if sys.platform == "win32" else "clear")
                print(f"Disabling module {moduleData[moduleId].Name}...")
                shutil.move(moduleData[moduleId].FolderPath, "Syati/DisabledModules/")
                moduleData[moduleId].FolderPath.replace("/Modules/", "/DisabledModules/")
                moduleData[moduleId].ModuleType = "disabled"
                print("Done.")
            case "x":
                print("Are you sure you want to remove this module?")
                if (input("[Y/N] ").lower() == "y"):
                    os.system("cls" if sys.platform == "win32" else "clear")
                    print(f"Deleting module {moduleData[moduleId].Name}...")
                    subprocess.call(f"rm -rf \"{moduleData[moduleId].FolderPath}\"")
                    del moduleData[moduleId]
                    print("Done.")
    elif (moduleData[moduleId].ModuleType == "disabled"):
        print("\033[91mDisabled\033[0m")
        print(f"Name: {moduleData[moduleId].Name}")
        print(f"Author: {moduleData[moduleId].Author}")
        print(f"Description: {moduleData[moduleId].Description}")
        match (input("[1] = Enable, [X] = Remove, [C] = Cancel: ").lower()):
            case "1":
                os.system("cls" if sys.platform == "win32" else "clear")
                print(f"Enabling module {moduleData[moduleId].Name}...")
                if (not checkDependencies(moduleData[moduleId])):
                    print("Abort.")
                    return
                shutil.move(moduleData[moduleId].FolderPath, "Syati/Modules/")
                moduleData[moduleId].FolderPath.replace("/DisabledModules/", "/Modules/")
                moduleData[moduleId].ModuleType = "enabled"
                print("Done.")
            case "x":
                print("Are you sure you want to remove this module?")
                if (input("[Y/N] ").lower() == "y"):
                    os.system("cls" if sys.platform == "win32" else "clear")
                    print(f"Deleting module {moduleData[moduleId].Name}...")
                    subprocess.call(f"rm -rf \"{moduleData[moduleId].FolderPath}\"")
                    del moduleData[moduleId]
                    print("Done.")
    else:
        print("\033[94mNot installed\033[0m")
        print(f"Name: {moduleData[moduleId].Name}")
        print(f"Author: {moduleData[moduleId].Author}")
        print(f"Description: {moduleData[moduleId].Description}")
        print(f"InstallType: {moduleData[moduleId].InstallType}")
        match (input("[I] = Install, [C] = Cancel: ").lower()):
            case "i":
                os.system("cls" if sys.platform == "win32" else "clear")
                installModule(moduleData[moduleId])

def createModuleInfoFromJson (moduleJson :object):
    moduleInfo = ModuleInfo()
    moduleInfo.Name = moduleJson.get("Name")
    moduleInfo.Author = moduleJson.get("Author")
    moduleInfo.Description = moduleJson.get("Description")
    moduleInfo.InstallDependencies = moduleJson.get("InstallDependencies", list())
    moduleInfo.BuildTasks = moduleJson.get("BuildTasks", list())
    moduleInfo.ObjectDatabaseEntries = moduleJson.get("ObjectDatabaseEntries", list())
    return moduleInfo

def createInstallableModuleFromJson (moduleJson :object):
    moduleInfo = InstallableModule()
    moduleInfo.Name = moduleJson.get("Name")
    moduleInfo.Author = moduleJson.get("Author")
    moduleInfo.Description = moduleJson.get("Description")
    moduleInfo.InstallType = moduleJson.get("InstallType")
    moduleInfo.InstallUrl = moduleJson.get("InstallUrl")
    moduleInfo.GitRepo = moduleJson.get("GitRepo")
    moduleInfo.GitPath = moduleJson.get("GitPath")
    moduleInfo.FolderName = moduleJson.get("FolderName")
    return moduleInfo

def checkIfModuleIsInstalled (moduleName :str):
    global moduleData
    for module in moduleData:
        if module.ModuleType == "available":
            return False
        if module.Name == moduleName:
            return True
    return False

def initModules (printOutInfo :bool = True, includeDuplicates :bool = False):
    global moduleData
    global installableModules
    moduleData = list()
    totalModuleAmount = 0
    if (printOutInfo):
        print("--- Enabled Modules: ---")
    enabledModuleAmount = 0
    for module in os.listdir("Syati/Modules"):
        if module[0] == ".": # ignore modules with a . in front of them
            continue
        with open("Syati/Modules/" + module + "/ModuleInfo.json", "r") as infoJson:
            moduleJson = json.load(infoJson)
        moduleInfo = createModuleInfoFromJson(moduleJson)
        moduleInfo.ModuleType = "enabled"
        moduleInfo.FolderPath = "Syati/Modules/" + module
        moduleData.append(moduleInfo)
        if (printOutInfo):
            print(f"#{totalModuleAmount}: {moduleInfo.Name}")
        totalModuleAmount += 1
        enabledModuleAmount += 1
    if (not enabledModuleAmount and printOutInfo):
        print("No enabled modules.")
    if (printOutInfo):
        print("\n--- Disabled Modules: ---")
    disabledModuleAmount = 0
    for module in os.listdir("Syati/DisabledModules"):
        if "." in module:
            continue
        with open("Syati/DisabledModules/" + module + "/ModuleInfo.json", "r") as infoJson:
            moduleJson = json.load(infoJson)
        moduleInfo = createModuleInfoFromJson(moduleJson)
        moduleInfo.ModuleType = "disabled"
        moduleInfo.FolderPath = "Syati/DisabledModules/" + module
        moduleData.append(moduleInfo)
        if (printOutInfo):
            print(f"#{totalModuleAmount}: {moduleInfo.Name}")
        totalModuleAmount += 1
        disabledModuleAmount += 1
    if (not disabledModuleAmount and printOutInfo):
        print("No disabled modules.")
    if (printOutInfo):
        print("\n--- Available Modules: ---")
    installableModuleAmount = 0
    for module in installableModules:
        if (not includeDuplicates and checkIfModuleIsInstalled(module.get("Name"))):
            continue
        moduleInfo = InstallableModule()
        moduleInfo.Name = module.get("Name")
        moduleInfo.Author = module.get("Author")
        moduleInfo.Description = module.get("Description")
        moduleInfo.InstallType = module.get("InstallType")
        moduleInfo.InstallUrl = module.get("InstallUrl")
        moduleInfo.GitRepo = module.get("GitRepo")
        moduleInfo.GitPath = module.get("GitPath")
        moduleInfo.FolderName = module.get("FolderName")
        moduleInfo.ModuleType = "available"
        moduleData.append(moduleInfo)
        if (printOutInfo):
            print(f"#{totalModuleAmount}: {moduleInfo.Name}")
        totalModuleAmount += 1
        installableModuleAmount += 1
    if (not installableModuleAmount and printOutInfo):
        print("No available modules.")

def moduleManager ():
    while True:
        initModules()
        print("\n[0] - View Details for Module #0")
        print("[0,1,2] - Enable Modules #0, #1, #2")
        print("[N] - Create New Module")
        print("[C] - Cancel")
        actionStr = input().lower()
        if (actionStr == "c"):
            return
        elif (actionStr == "n"):
            newModule()
        elif (not actionStr.count(",")):
            showModuleDetails(int(actionStr))
        else:
            for currentModule in actionStr.split(","):
                currentModule = int(currentModule)
                if (moduleData[currentModule].ModuleType == "available"):
                    installModule(moduleData[currentModule])
                    continue
                elif (moduleData[currentModule].ModuleType == "enabled"):
                    continue

                if (not checkDependencies(moduleData[currentModule])):
                    print("Abort.")
                    return
                shutil.move(moduleData[currentModule].FolderPath, "Syati/Modules/")
                moduleData[currentModule].FolderPath.replace("/DisabledModules/", "/Modules/")
                moduleData[currentModule].ModuleType = "enabled"

def getInstallableModuleFromModuleName (moduleName :str):
    global installableModules
    for installableModule in installableModules:
        if (installableModule["Name"] == moduleName):
            return installableModule
    return False

def updateModules ():
    global moduleData
    os.system("cls" if sys.platform == "win32" else "clear")
    print("Updating all modules...")
    for module in moduleData:
        if module.ModuleType == "available":
            print("\nDone.")
            return
        print(f"{module.Name}: ", end="\n\t")
        if not os.path.isdir(f"{module.FolderPath}/.git"):
            installableModule = getInstallableModuleFromModuleName(module.Name)
            if (not installableModule):
                print("Warning: Could not find an install method for module. Skipping update...")
                continue
            if (installableModule["InstallType"] == "url_tar"):
                print("Redownloading module...")
                pathToModuleTar = f"{module.FolderPath}/../{os.path.basename(module.InstallUrl)}"
                with request.urlopen(module.InstallUrl) as req:
                    moduleTar = req.read()
                with open(pathToModuleTar, "wb") as f:
                    f.write(moduleTar)
                shutil.rmtree(module.FolderPath)
                with tarfile.open(pathToModuleTar) as f:
                    f.extractall(".")
                os.remove(pathToModuleTar)
                print("\tSuccess.")
            elif (installableModule["InstallType"] == "url_zip"):
                print("Redownloading module...")
                pathToModuleTar = f"{module.FolderPath}/../{os.path.basename(module.InstallUrl)}"
                with request.urlopen(module.InstallUrl) as req:
                    moduleTar = req.read()
                with open(pathToModuleTar, "wb") as f:
                    f.write(moduleTar)
                shutil.rmtree(module.FolderPath)
                with zipfile.ZipFile.open(pathToModuleTar) as f:
                    f.extractall(".")
                os.remove(pathToModuleTar)
                print("\tSuccess.")
            elif (installableModule["InstallType"] == "lginc_module"):
                print("Redownloading module...")
                with request.urlopen(f"https://mariogalaxy.org/syati-modules?module={installableModule["InstallUrl"]}") as req:
                    moduleTar = req.read()
                with open(installableModule["InstallUrl"] + ".tar.gz", "wb") as f:
                    f.write(moduleTar)
                shutil.rmtree(module.FolderPath)
                os.mkdir(installableModule["InstallUrl"])
                os.chdir(installableModule["InstallUrl"])
                with tarfile.open("../" + os.path.basename(installableModule["InstallUrl"]) + ".tar.gz") as f:
                    f.extractall(".")
                os.chdir("../../..")
                os.remove(installableModule["InstallUrl"] + ".tar.gz")
                print("\tSuccess.")
            else:
                print("\tWarning: Could not find an install method for module. Skipping update...")
        else:
            if (subprocess.run(["git", "-C", module.FolderPath, "pull"]).returncode):
                print(f"Error while updating {module}. Skipping update...")

print("Syati Manager v2.0\nby Bavario\n-----------------")
if not os.path.isdir("Syati/"):
    print("No instance of Syati was found. Please run SyatiSetup.py.")
    exit(1)

print("Getting newest database...")
try:
    argc = 0
    for argument in sys.argv:
        if argument == "--use-local":
            del sys.argv[argc]
            raise Exception
        argc += 1
    with request.urlopen("https://github.com/SMGCommunity/SyatiManager/raw/main/installable_modules.json") as req:
        installableJson = req.read()
    installableModules = json.loads(installableJson)
except:
    print("Using local module list. New modules may not be available.")
    installableModules = json.load(open("installable_modules.json", "r"))

if (len(sys.argv) > 1):
    outputPath = f"Syati/Output/{os.path.splitext(os.path.basename(sys.argv[1]))[0]}"
    for module in os.listdir("Syati/Modules"):
        shutil.move(f"Syati/Modules/{module}", f"Syati/DisabledModules/{module}")
    print(f"Building solution {os.path.basename(outputPath)}...")
    initModules(False, True)
    with open(sys.argv[1], "r") as f:
        solutionData = json.load(f)
    installAll = (True if "InstallAll" in solutionData and solutionData["InstallAll"] else False)
    for module in solutionData["Modules"]:
        if (installAll or not os.path.isdir(f"Syati/DisabledModules/{module}")):
            installableModule = getInstallableModuleFromFolderName(module)
            if (not installableModule):
                print(f"Fatal: No way to install module {module} was found.")
                exit(1)
            if (installModule(installableModule, True) == "disabled"):
                print(f"Solution {os.path.basename(outputPath)} cannot be built.")
        elif (os.path.isdir(f"Syati/DisabledModules/{module}")):
            print(f"Enabling module {module}...")
            shutil.move(f"Syati/DisabledModules/{module}", f"Syati/Modules/{module}")
    initModules(False)
    if ("LocalModules" not in solutionData):
        solutionData["LocalModules"] = []
    for localModule in solutionData["LocalModules"]:
        print(f"Copying local module {os.path.basename(localModule)}...")
        shutil.copytree(f"{os.path.dirname(sys.argv[1])}/{localModule}", f"Syati/Modules/{os.path.basename(localModule)}")
    print("\nAll required modules enabled. Building...")
    if ("OutputPath" not in solutionData):
        solutionData["OutputPath"] = "Syati/Output"
    else:
        path = Path(sys.argv[1])
        solutionData["OutputPath"] = (str(path.parent) if path.is_file() else str(path)) + "/" + solutionData["OutputPath"]
    if ("BuildTasks" not in solutionData):
        solutionData["BuildTasks"] = list()
    if ("Unibuild" not in solutionData):
        solutionData["Unibuild"] = False
    else:
        print("Using unibuild.")
    if (not buildScript(solutionData["Regions"], solutionData["BuildTasks"], solutionData["OutputPath"], solutionData["Unibuild"])):
        print(f"Solution {os.path.basename(outputPath)} cannot be built.")
        exit(1)
    print(f"Solution {os.path.basename(outputPath)} built successfully!")
    exit(0)

while True:
    print("Would you like to build [B], compile the loader [L], manage modules [M], update modules [U] or quit [Q]?")
    match (input().lower()):
        case "b":
            initModules(False)
            buildScript()
            continue
        case "l":
            buildLoaderScript()
            continue
        case "m":
            os.system("cls" if sys.platform == "win32" else "clear")
            moduleManager()
            continue
        case "u":
            initModules(False)
            updateModules()
        case "q":
            exit(0)