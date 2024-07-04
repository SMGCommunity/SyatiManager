from zipfile import ZipFile
from urllib import request
from io import BytesIO
import subprocess, os, sys

CW_WIN32 = "https://mariogalaxy.org/CodeWarrior-Syati.zip"
CW_LINUX = "https://mariogalaxy.org/mwcceppc-syati"
CWASM_LINUX = "https://mariogalaxy.org/mwasmeppc-syati"
KAMEK = "https://github.com/Treeki/Kamek/releases/download/2024-04-10_prerelease/kamek_2024-04-10_win-x64.zip"
SYATI = "https://github.com/SMGCommunity/Syati"
SMBT_WIN32 = "https://github.com/SMGCommunity/SyatiModuleBuildTool/releases/latest/download/SyatiModuleBuildTool.exe"
SMBT_DARWIN = "https://github.com/SMGCommunity/SyatiModuleBuildTool/releases/latest/download/SyatiModuleBuildTool-macos"
SMBT_LINUX = "https://github.com/SMGCommunity/SyatiModuleBuildTool/releases/latest/download/SyatiModuleBuildTool-linux"
TEMPLATE = "https://github.com/SMGCommunity/SyatiModuleTemplate"

def Download_CW():
    print("Downloading CodeWarrior Compiler...")
    if (sys.platform == "win32"):
        with request.urlopen(CW_WIN32) as req:
            data = BytesIO(req.read())
        with ZipFile(data) as zip:
            zip.extractall("CodeWarrior")
    else:
        with request.urlopen(CW_LINUX) as req:
            cwData = req.read()
        with open("CodeWarrior/mwcceppc", "wb") as f:
            f.write(cwData)
        with request.urlopen(CWASM_LINUX) as req:
            cwData = req.read()
        with open("CodeWarrior/mwasmeppc", "wb") as f:
            f.write(cwData)
        os.chmod('mwcceppc', 0o755)
        os.chmod('mwasmeppc', 0o755)

def Download_Kamek():
    print("Downloading Kamek Linker...")
    with request.urlopen(KAMEK) as req:
        data = BytesIO(req.read())
    with ZipFile(data) as zip:
        zip.extractall("Kamek")

def Clone_Syati():
    print("Cloning Syati...")
    code = subprocess.call(["git", "clone", SYATI], stderr=open(os.devnull, "wb"))
    if code != 0:
        print("Cloning Syati failed.")
        exit(1)

def Clone_Template():
    print("Cloning SyatiModuleTemplate...")
    code = subprocess.call(["git", "clone", TEMPLATE], stderr=open(os.devnull, "wb"))
    if code != 0:
        print("Cloning SyatiModuleTemplate failed.")
        exit(1)

def Download_SMBT():
    print("Downloading SyatiModuleBuildTool...")
    if (sys.platform == "win32"):
        smbtUrl = SMBT_WIN32
    elif (sys.platform == "darwin"):
        smbtUrl = SMBT_DARWIN
    else:
        smbtUrl = SMBT_LINUX
    with request.urlopen(smbtUrl) as req:
        smbtData = req.read()
    with open("SyatiModuleBuildTool", "wb") as f:
        f.write(smbtData)

if __name__ == "__main__":
    if os.path.exists("Syati"):
        print("An existing Syati folder was found. Please run SyatiManager.py or delete the folder.")
        exit(1)
    if (subprocess.call(["git"], stdout=open(os.devnull, "wb")) != 1):
        if sys.platform == "win32":
            print("Git was not found. Would you like to download it now?")
            prompt = input("[Y/N] ")
            if (prompt.lower() == "y" & subprocess.call(["winget", "install", "git.git"]) != 1):
                print("Error while installing Git.")
                exit(2)
            else:
                exit(2)
        else:
            print("Git was not found. Please download it using your standard package manager.")
            exit(2)

    os.makedirs("Syati")
    os.chdir("Syati")
    Clone_Syati()
    Download_SMBT()
    os.chdir("Syati/deps")
    Download_CW()
    Download_Kamek()
    os.chdir("../..")
    Clone_Template()
    os.makedirs("Modules")
    os.makedirs("DisabledModules")
    os.makedirs("Output")
    os.chdir("..")
    print("Done.")