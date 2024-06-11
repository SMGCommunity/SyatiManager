import os
import sys
import subprocess
from urllib import request
from urllib.parse import urlparse
import json
import tarfile
import zipfile
import shutil
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

def wingetInstall (packageName, displayName):
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

def buildScript ():
    global moduleData
    if (sys.platform == "win32"):
        os.system("cls")
    else:
        os.system("clear")
        if (subprocess.run("wine", stderr=open(os.devnull, "wb")).returncode):
            print("wine was not found. Please download it using your standard package manager.")
            return
    print("Which region? Press Enter to build all regions.")
    match (input("[J] = JPN, [U] = USA, [P] = PAL, [K] = KOR, [T] = TWN: ").lower()):
        case "j":
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "JPN", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
        case "u":
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "USA", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
        case "p":
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "PAL", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
        case "k":
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "KOR", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
        case "t":
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "TWN", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
        case _:
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "JPN", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
            if (exitCode):
                print("Error while building target JPN. Continue building?")
                if (input("[Y/N] ").lower() == "n"):
                    return
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "USA", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
            if (exitCode):
                print("Error while building target USA. Continue building?")
                if (input("[Y/N] ").lower() == "n"):
                    return
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "PAL", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
            if (exitCode):
                print("Error while building target PAL. Continue building?")
                if (input("[Y/N] ").lower() == "n"):
                    return
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "KOR", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
            if (exitCode):
                print("Error while building target KOR. Continue building?")
                if (input("[Y/N] ").lower() == "n"):
                    return
            exitCode = subprocess.run(["./Syati/SyatiModuleBuildTool", "TWN", "Syati/Syati/", "Syati/Modules/", "Syati/Output/"]).returncode
            if (exitCode):
                print("Error while building target TWN. Continue building?")
                if (input("[Y/N] ").lower() == "n"):
                    return
    if (exitCode):
        print("\nError while building. Abort.")
    else:
        objectdbPath = ""
        for module in moduleData:
            if (module.ModuleType != "enabled"):
                break
            if os.path.isdir("Syati/Output/disc"):
                shutil.rmtree("Syati/Output/disc")
            for buildTask in module.BuildTasks:
                print(f"Running build task `{buildTask}`")
                if (buildTask == "SyatiManager_copydisc"):
                    shutil.copytree(module.FolderPath + "/disc", "Syati/Output/disc")
                else:
                    subprocess.run(buildTask)
            for objectdbEntry in module.ObjectDatabaseEntries:
                if (not objectdbPath):
                    objectdbPath = input("Please specify the path to Whitehole's objectdb.json: ")
                with open(objectdbPath, "r") as f:
                    objectdbData = json.load(f)
                objectdbData.append(objectdbEntry)
                with open(objectdbPath, "w") as f:
                    json.dump(objectdbData, f)
        print("\nBuilt successfully!")
        if os.path.isdir("Syati/Output/disc"):
            print("Note: Make sure to copy the disc/ folder into your game dump, otherwise the built modules may not work correctly.")

def buildLoaderScript ():
    if (sys.platform == "win32"):
        os.system("cls")
    else:
        os.system("clear")
        if (subprocess.run("wine", stderr=open(os.devnull, "wb")).returncode):
            print("wine was not found. Please download it using your standard package manager.")
            return
    makeFullXML = False
    print("Full [F] or Partial [P] XML?")
    if (input("If you do not use a Riivolution XML yet, choose Full. ").lower() == "f"):
        makeFullXML = True
    print("Which region? Press Enter to build all regions.")
    os.chdir("Syati/Syati")
    match (input("[J] = JPN, [U] = USA, [P] = PAL, [K] = KOR, [T] = TWN: ").lower()):
        case "j":
            exitCode = subprocess.run(f"python buildloader.py JPN -o ../Output/ {"--full-xml" if makeFullXML else ""}").returncode
        case "u":
            exitCode = subprocess.run(f"python buildloader.py USA -o ../Output/ {"--full-xml" if makeFullXML else ""}").returncode
        case "p":
            exitCode = subprocess.run(f"python buildloader.py PAL -o ../Output/ {"--full-xml" if makeFullXML else ""}").returncode
        case "k":
            exitCode = subprocess.run(f"python buildloader.py KOR -o ../Output/ {"--full-xml" if makeFullXML else ""}").returncode
        case "t":
            exitCode = subprocess.run(f"python buildloader.py TWN -o ../Output/ {"--full-xml" if makeFullXML else ""}").returncode
        case _:
            exitCode = subprocess.run(["python", "buildloader.py", "-o", "../Output/", ("--full-xml" if makeFullXML else "")]).returncode
    os.chdir("../..")
    if (exitCode):
        print("\nError while building the loader. Abort.")
    else:
        print("\nBuilt the loader successfully!")

def installModule (module):
    global moduleData
    os.system("cls" if sys.platform == "win32" else "clear")
    match (module.InstallType):
        case "git" | "git_recursive":
            if (subprocess.run(["git"], stdout=open(os.devnull, "wb")).returncode != 1):
                if (not wingetInstall("git", "Git")):
                    return False
            print(f"Cloning module {module.Name}...")
            if (subprocess.run(["git", "-C", "Syati/Modules", "clone", module.InstallUrl + (" --recursive" if module.InstallType == "git_recursive" else "")], stderr=open(os.devnull, "wb")).returncode):
                print(f"Cloning module {module.Name} failed. Abort.")
                return False
        case "git_folder":
            print(f"Downloading module {module.Name}...")
            pathToModuleTar = "Syati/Modules/" + os.path.basename(module.GitPath)
            with request.urlopen(f"https://mariogalaxy.org/github2tar?repo={module.GitRepo}&path={module.GitPath}") as req:
                moduleTar = req.read()
            with open(pathToModuleTar + ".tar.xz", "wb") as f:
                f.write(moduleTar)
            os.mkdir(pathToModuleTar)
            os.chdir(pathToModuleTar)
            with tarfile.open("../" + os.path.basename(pathToModuleTar) + ".tar.xz") as f:
                f.extractall(".")
            os.chdir("../../..")
            os.remove(pathToModuleTar + ".tar.xz")
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
    if (not checkDependencies(moduleInfo)):
        shutil.move(module.FolderPath, "Syati/DisabledModules/")
        module.FolderPath.replace("/Modules/", "/DisabledModules/")
        module.ModuleType = "disabled"
    print("Done.")
    return True

def getModuleFromFolderPath (moduleFolderPath):
    global moduleData
    for module in moduleData:
        if (module.ModuleType == "available"):
            continue
        if (module.FolderPath == moduleFolderPath):
            return module
    return False

def getInstallableModuleFromFolderName (moduleFolderName):
    global moduleData
    for module in moduleData:
        if (module.ModuleType != "available"):
            continue
        if (module.FolderName == moduleFolderName):
            return module
    return False

def checkDependencies (module):
    for dependency in module.InstallDependencies:
        dependencyData = getModuleFromFolderPath("Syati/DisabledModules/" + dependency)
        if (dependencyData):
            print(f"This module requires the disabled module {dependencyData.Name}. Enable it now?")
            if (input("[Y/N] ").lower() == "y"):
                if (not checkDependencies(dependencyData)):
                    print("Abort.")
                    return False
                shutil.move(dependencyData.FolderPath, "Syati/Modules/")
                dependencyData.FolderPath.replace("/DisabledModules/", "/Modules/")
                dependencyData.ModuleType = "enabled"
            else:
                return False
        else:
            dependencyData = getModuleFromFolderPath("Syati/Modules/" + dependency)
            if (not dependencyData):
                dependencyData = getInstallableModuleFromFolderName(dependency)
                if (not dependencyData):
                    print(f"Fatal error: Module {dependency} is required but was not found.")
                    return False
                print(f"This module requires the installable module {dependency}. Download it now?")
                if (input("[Y/N] ").lower() == "y"):
                    installModule(dependencyData)
                else:
                    return False
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

def showModuleDetails (moduleId):
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
                installModule(moduleData[moduleId])

def createModuleInfoFromJson (moduleJson):
    moduleInfo = ModuleInfo()
    moduleInfo.Name = moduleJson.get("Name")
    moduleInfo.Author = moduleJson.get("Author")
    moduleInfo.Description = moduleJson.get("Description")
    moduleInfo.InstallDependencies = moduleJson.get("InstallDependencies", list())
    moduleInfo.BuildTasks = moduleJson.get("BuildTasks", list())
    moduleInfo.ObjectDatabaseEntries = moduleJson.get("ObjectDatabaseEntries", list())
    return moduleInfo

def createInstallableModuleFromJson (moduleJson):
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

def checkIfModuleIsInstalled (moduleName):
    global moduleData
    for module in moduleData:
        if module.Name == moduleName:
            return True
    return False

def moduleManager ():
    global moduleData
    global installableModules
    while True:
        moduleData = list()
        totalModuleAmount = 0
        print("--- Enabled Modules: ---")
        enabledModuleAmount = 0
        for module in os.listdir("Syati/Modules"):
            if "." in module:
                continue
            with open("Syati/Modules/" + module + "/ModuleInfo.json", "r") as infoJson:
                moduleJson = json.load(infoJson)
            moduleInfo = createModuleInfoFromJson(moduleJson)
            moduleInfo.ModuleType = "enabled"
            moduleInfo.FolderPath = "Syati/Modules/" + module
            moduleData.append(moduleInfo)
            print(f"#{totalModuleAmount}: {moduleInfo.Name}")
            totalModuleAmount += 1
            enabledModuleAmount += 1
        if (not enabledModuleAmount):
            print("No enabled modules.")
        print("\n--- Disabled Modules: ---")
        disabledModuleAmount = 0
        for module in os.listdir("Syati/DisabledModules"):
            with open("Syati/DisabledModules/" + module + "/ModuleInfo.json", "r") as infoJson:
                moduleJson = json.load(infoJson)
            moduleInfo = createModuleInfoFromJson(moduleJson)
            moduleInfo.ModuleType = "disabled"
            moduleInfo.FolderPath = "Syati/DisabledModules/" + module
            moduleData.append(moduleInfo)
            print(f"#{totalModuleAmount}: {moduleInfo.Name}")
            totalModuleAmount += 1
            disabledModuleAmount += 1
        if (not disabledModuleAmount):
            print("No disabled modules.")
        print("\n--- Available Modules: ---")
        installableModuleAmount = 0
        for module in installableModules:
            if (checkIfModuleIsInstalled(module.get("Name"))):
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
            print(f"#{totalModuleAmount}: {moduleInfo.Name}")
            totalModuleAmount += 1
            installableModuleAmount += 1
        if (not installableModuleAmount):
            print("No available modules.")
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

def hasGitUpdates (dir):
    if (not os.path.isdir(f"{dir}/.git")):
        return False
    proc = subprocess.run(f"git -C \"{dir}\" rev-parse FETCH_HEAD", stdout=subprocess.PIPE)
    fetch_head = proc.stdout.decode().strip()
    proc = subprocess.run(f"git -C \"{dir}\" rev-parse HEAD", stdout=subprocess.PIPE)
    head = proc.stdout.decode().strip()
    return head != fetch_head

def updateModules ():
    global moduleData
    updateAll = False
    for module in os.listdir("Syati/Modules"):
        if (hasGitUpdates("Syati/Modules/" + module) and not updateAll):
            print(f"{module} has updates available.")
            action = input("[A] = Update All, [U] = Update, [X] = Do Not Update: ")
            if (action.lower() == "x"):
                break
            elif (action.lower() == "a"):
                updateAll = True
        elif (hasGitUpdates("Syati/Modules/" + module) and subprocess.run(["git", "-C", "Syati/Modules/" + module, "pull"], stderr=open(os.devnull, "wb")).returncode):
            print(f"Error while updating {module}. Skipping update...")

print("Syati Manager v2.0\nby Bavario\n-----------------")
if not os.path.isdir("Syati/"):
    print("No instance of Syati was found. Please run SyatiSetup.py.")
    exit(1)

print("Getting newest database...")
try:
    with request.urlopen("https://github.com/SMGCommunity/SyatiManager/raw/main/installable_modules.json") as req:
        installableJson = req.read()
    installableModules = json.loads(installableJson)
except:
    print("Failed to fetch module list. New modules may not be available.")
    installableModules = json.load(open("installable_modules.json", "r"))
print("Checking for module updates...")
updateModules()

while True:
    print("Would you like to build [B], compile the loader [L], manage modules [M] or quit [Q]?")
    match (input().lower()):
        case "b":
            buildScript()
            continue
        case "l":
            buildLoaderScript()
            continue
        case "m":
            os.system("cls" if sys.platform == "win32" else "clear")
            moduleManager()
            continue
        case "q":
            exit(0)