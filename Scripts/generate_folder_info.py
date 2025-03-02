import os
import glob
import json

def genInfo(folder_path: str):
    if not os.path.isdir(folder_path):
        print(f"\033[91mError: \"{folder_path}\" is not a directory.\033[0m")
        return
    
    infoPath: str = os.path.join(folder_path, "GitFolderInfo.json")

    if os.path.isfile(infoPath):
        print(f"{folder_path} already has a folder info file.")
        return

    files: list = []

    for file in glob.glob(folder_path + "/**", recursive=True):
        if not os.path.isfile(file):
            continue
        
        path = os.path.relpath(file, folder_path).replace("\\", "/")

        if path == "ModuleInfo.json" or path == "GitFolderInfo.json":
            continue

        files.append(path)

    json_content: dict = {
        "Version": 1,
        "Files": files
    }

    with open(os.path.join(folder_path, "GitFolderInfo.json"), "w") as file:
        file.write(json.dumps(json_content, indent=2))

    print(f"Generated folder info for {folder_path}, {len(files)} files.")

if __name__ == "__main__":
    # If you are working in an environment where there are multiple folder modules,
    # I suggest you copy this script and replace the while loop below the following code:
    
    # for dir in glob.glob("ModulesPath\*", recursive=False):
    #     if os.path.isdir(dir):
    #         gen(dir)

    while True:
        genInfo(input("Input the folder path: "))