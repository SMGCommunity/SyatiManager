@ECHO OFF
SETLOCAL EnableDelayedExpansion
TITLE Syati Manager

ECHO Syati Manager v1.0
ECHO by Bavario
ECHO -----------------
IF EXIST Syati/ GOTO MainSection


:SetupScript
CHOICE /M "This script will now download Syati and its dependencies into Syati/. This may take a while. Would you like to proceed" /C YN
IF %ERRORLEVEL% EQU 2 GOTO :End
ECHO.
GIT >NUL
IF %ERRORLEVEL% GTR 1 (
    CHOICE /M "Git was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :End
    WINGET INSTALL GIT.GIT
    ECHO.
)
CURL --help >NUL
IF %ERRORLEVEL% GTR 0 (
    CHOICE /M "cURL was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :End
    WINGET INSTALL CURL.CURL
    ECHO.
)

MKDIR Syati && CD Syati
ECHO Cloning Syati...
GIT clone https://github.com/SMGCommunity/Syati/ 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while cloning Syati.
    GOTO :End
)
ECHO Downloading SyatiModuleBuildTool...
CURL -k -L -O https://github.com/SMGCommunity/SyatiModuleBuildTool/releases/latest/download/SyatiModuleBuildTool.exe 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while downloading SyatiBuildModuleTool.
    GOTO :End
)
CD Syati/deps
ECHO Downloading SyatiSetup.exe...
CURL -k -L -O https://github.com/Lord-Giganticus/SyatiSetup/releases/download/Auto/syatisetup.exe 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while downloading SyatiSetup.exe.
    GOTO :End
)
ECHO Running SyatiSetup.exe...
SyatiSetup.exe >NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while running SyatiSetup.exe.
    GOTO :End
)
RM -rf SyatiSetup.exe
CD ../..

MKDIR Modules && CD Modules
ECHO Cloning Syati_Init...
GIT clone https://github.com/SMGCommunity/Syati_Init 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while cloning Syati_Init.
    GOTO :End
)
ECHO Cloning Syati_ObjectFactories...
GIT clone https://github.com/SMGCommunity/Syati_ObjectFactories 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while cloning Syati_ObjectFactories.
    GOTO :End
)
ECHO Cloning Syati_CommonFunctionHooks...
GIT clone https://github.com/SMGCommunity/Syati_CommonFunctionHooks 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while cloning Syati_CommonFunctionHooks.
    GOTO :End
)
CD ..
ECHO Cloning SyatiModuleTemplate...
GIT clone https://github.com/SMGCommunity/SyatiModuleTemplate 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while cloning SyatiModuleTemplate.
    GOTO :End
)
MKDIR Output
CD ../
ECHO Done.


:MainSection
TITLE Syati Manager - Main Menu
ECHO Getting newest csv...
CURL -k -L -O https://github.com/SMGCommunity/SyatiManager/raw/main/installable_modules.csv 2>NUL
IF %ERRORLEVEL% GTR 1 ECHO Error %ERRORLEVEL% while getting newest csv. New modules may not be available.
ECHO.
CHOICE /M "Would you like to build [B], compile the loader [L], manage modules [M] or quit [Q]?" /C BLMQ /N
IF %ERRORLEVEL% EQU 1 GOTO BuildScript
IF %ERRORLEVEL% EQU 2 GOTO LoaderScript
IF %ERRORLEVEL% EQU 3 GOTO ModuleManager
IF %ERRORLEVEL% EQU 4 GOTO :End


:BuildScript
TITLE Syati Manager - Building
CHOICE /M "Which region? (ALL, JPN, USA, PAL, KOR, TWN)" /C AJUPKT /N
IF %ERRORLEVEL% EQU 1 (
    ECHO Building JPN...
    Syati\SyatiModuleBuildTool.exe JPN Syati/Syati/ Syati/Modules/ Syati/Output/ >NUL
    IF !ERRORLEVEL! NEQ 0 (
        ECHO An error occured while trying to build JPN.
        GOTO MainSection
    )
    ECHO Building USA...
    Syati\SyatiModuleBuildTool.exe USA Syati/Syati/ Syati/Modules/ Syati/Output/ >NUL
    IF !ERRORLEVEL! NEQ 0 (
        ECHO An error occured while trying to build USA.
        GOTO MainSection
    )
    ECHO Building PAL...
    Syati\SyatiModuleBuildTool.exe PAL Syati/Syati/ Syati/Modules/ Syati/Output/ >NUL
    IF !ERRORLEVEL! NEQ 0 (
        ECHO An error occured while trying to build PAL.
        GOTO MainSection
    )
    ECHO Building KOR...
    Syati\SyatiModuleBuildTool.exe KOR Syati/Syati/ Syati/Modules/ Syati/Output/ >NUL
    IF !ERRORLEVEL! NEQ 0 (
        ECHO An error occured while trying to build KOR.
        GOTO MainSection
    )
    ECHO Building TWN...
    Syati\SyatiModuleBuildTool.exe TWN Syati/Syati/ Syati/Modules/ Syati/Output/ >NUL
    IF !ERRORLEVEL! NEQ 0 (
        ECHO An error occured while trying to build TWN.
        GOTO MainSection
    )
)
IF %ERRORLEVEL% EQU 2 Syati\SyatiModuleBuildTool.exe JPN Syati/Syati/ Syati/Modules/ Syati/Output/
IF %ERRORLEVEL% EQU 3 Syati\SyatiModuleBuildTool.exe USA Syati/Syati/ Syati/Modules/ Syati/Output/
IF %ERRORLEVEL% EQU 4 Syati\SyatiModuleBuildTool.exe PAL Syati/Syati/ Syati/Modules/ Syati/Output/
IF %ERRORLEVEL% EQU 5 Syati\SyatiModuleBuildTool.exe KOR Syati/Syati/ Syati/Modules/ Syati/Output/
IF %ERRORLEVEL% EQU 6 Syati\SyatiModuleBuildTool.exe TWN Syati/Syati/ Syati/Modules/ Syati/Output/
IF !ERRORLEVEL! NEQ 0 (
    ECHO.
    ECHO An error occured while trying to build.
    ECHO [R] = Retry, [C] = Cancel
    CHOICE /C RC /N
    IF !ERRORLEVEL! EQU 1 CLS && GOTO BuildScript
    IF !ERRORLEVEL! EQU 2 CLS && GOTO MainSection
)
ECHO.
ECHO Built successfully
IF EXIST Syati\Output\disc ECHO Note: Make sure to copy the disc/ folder into your game dump, otherwise the built modules may not work correctly.
GOTO MainSection

:LoaderScript
TITLE Syati Manager - Compiling the loader
CD Syati\Syati
PYTHON --version >NUL
IF %ERRORLEVEL% NEQ 0 (
    CHOICE /M "Python was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL PYTHON3
    ECHO.
)
CHOICE /M "Which region? (ALL, JPN, USA, PAL, KOR, TWN)" /C AJUPKT /N
IF %ERRORLEVEL% EQU 1 (
    python buildloader.py -o ../Output/
    IF !ERRORLEVEL! NEQ 0 (
        ECHO An error occured while trying to compile the loader.
        ECHO [R] = Retry, [C] = Cancel
        CHOICE /C RC /N
        IF !ERRORLEVEL! EQU 1 CLS && GOTO LoaderScript
        IF !ERRORLEVEL! EQU 2 CLS && GOTO MainSection
    )
)
IF %ERRORLEVEL% EQU 2 python buildloader.py JPN -o ../Output/
IF %ERRORLEVEL% EQU 3 python buildloader.py USA -o ../Output/
IF %ERRORLEVEL% EQU 4 python buildloader.py PAL -o ../Output/
IF %ERRORLEVEL% EQU 5 python buildloader.py KOR -o ../Output/
IF %ERRORLEVEL% EQU 6 python buildloader.py TWN -o ../Output/
CD ../../
IF !ERRORLEVEL! NEQ 0 (
    ECHO.
    ECHO An error occured while trying to compile the loader.
    ECHO [R] = Retry, [C] = Cancel
    CHOICE /C RC /N
    IF !ERRORLEVEL! EQU 1 CLS && GOTO LoaderScript
    IF !ERRORLEVEL! EQU 2 CLS && GOTO MainSection
)
ECHO.
ECHO Compiled successfully
GOTO MainSection


:ModuleManager
TITLE Syati Manager - Module Manager
CLS
SET ChoiceStr=
SET ModuleList=
SET IncrementTotal=0
SET IncrementLocal=0
SET IncrementLocalDisabled=0
SET IncrementInstallable=0
ECHO --- Enabled Modules: ---
ECHO.
FOR /r "Syati\Modules" %%f IN (*) DO (
    IF "%%~nxf"=="ModuleInfo.json" (
        CALL :GetModuleName "%%f"
        ECHO #!IncrementTotal!: !Name!
        SET ChoiceStr=!ChoiceStr!!IncrementTotal!
        SET ModuleList=!ModuleList!!Name!,
        SET /A IncrementTotal=!IncrementTotal!+1
        SET /A IncrementLocal=!IncrementLocal!+1
    )
)
IF !IncrementLocal! EQU 0 ECHO No installed modules.
ECHO.
ECHO --- Disabled Modules: ---
ECHO.
FOR /r "Syati\DisabledModules" %%f IN (*) DO (
    IF "%%~nxf"=="ModuleInfo.json" (
        CALL :GetModuleName "%%f"
        ECHO #!IncrementTotal!: !Name!
        SET ChoiceStr=!ChoiceStr!!IncrementTotal!
        SET ModuleList=!ModuleList!!Name!,
        SET /A IncrementTotal=!IncrementTotal!+1
        SET /A IncrementLocal=!IncrementLocal!+1
        SET /A IncrementLocalDisabled=!IncrementLocal!+1
    )
)
IF !IncrementLocalDisabled! EQU 0 ECHO No disabled modules.
ECHO.
ECHO --- Available Modules: ---
ECHO.
FOR /f "tokens=1,* delims=;" %%f IN (installable_modules.csv) DO (
    ECHO !ModuleList! | FINDSTR /C:"%%f" 1>NUL
    IF ERRORLEVEL 1 (
        ECHO #!IncrementTotal!: %%f
        SET ChoiceStr=!ChoiceStr!!IncrementTotal!
        SET ModuleList=!ModuleList!%%f,
        SET /A IncrementTotal=!IncrementTotal!+1
        SET /A IncrementInstallable=!IncrementInstallable!+1
    )
)
IF !IncrementInstallable! EQU 0 ECHO No available modules.
ECHO.
ECHO [0] - View Details for Module #0
ECHO [0,1,2] - Enable Modules #0, #1 and #2
ECHO [C] - Cancel
SET /P ModuleStr=
IF "%ModuleStr%" EQU "C" GOTO MainSection
IF "%ModuleStr%" EQU "c" GOTO MainSection
FOR /F "tokens=1,* delims=," %%A IN ("%ModuleStr%") DO (
    IF "%%B" EQU "" (
        SET ModuleId=%%A
        GOTO :ViewModuleDetails
    ) ELSE FOR %%I IN (%%A,%%B,) DO (
        CALL :GetNameOfJSON %%I
        IF !ERRORLEVEL! EQU 2 CALL :EnableModule
        IF !ERRORLEVEL! EQU 3 (
            CALL :FindModuleFromList %%I
            IF "!InstallType!" EQU "git" CALL :InstallModuleGit
            IF "!InstallType!" EQU "git_r" CALL :InstallModuleGitRecursive
            IF "!InstallType!" EQU "curl_uncompressed" CALL :InstallModuleCurlUncompressed
            IF "!InstallType!" EQU "curl_compressed" CALL :InstallModuleCurlCompressed
            IF "!InstallType!" EQU "git_folder" CALL :InstallModuleGitFolder
        )
    )
)
GOTO :ModuleManager

:ViewModuleDetails
SET folderName=
SET /A Target=%ModuleId%
CLS
CALL :GetNameOfJSON !Target!
IF %ERRORLEVEL% EQU 1 (
    SET /A Target=%~1-1
    CALL :GetModuleData "!jsonName!" !Target!
    COLOR 0A
    ECHO Enabled
    ECHO Name: !Name!
    ECHO Author: !Author!
    ECHO Description: !Description!
    ECHO !jsonName! | FINDSTR /C:"PTD" 1>NUL
    IF NOT ERRORLEVEL 1 ECHO This module is part of PTD.
    ECHO.
    ECHO [0] = Disable, [X] = Remove, [C] = Cancel
    CHOICE /C 0XC /N
    IF !ERRORLEVEL! EQU 1 CALL :DisableModule
    IF !ERRORLEVEL! EQU 2 (
        ECHO Are you sure you want to remove this module?
        CHOICE /C YN
        IF !ERRORLEVEL! EQU 1 CALL :RemoveModule
    )
    IF !ERRORLEVEL! EQU 3 (
        COLOR 0F
        GOTO :ModuleManager
    )
) ELSE IF %ERRORLEVEL% EQU 2 (
    COLOR 0C
    SET /A Target=%~1-1
    CALL :GetModuleData "!jsonName!" !Target!
    ECHO Disabled
    ECHO Name: !Name!
    ECHO Author: !Author!
    ECHO Description: !Description!
    ECHO !jsonName! | FINDSTR /C:"PTD" 1>NUL
    IF NOT ERRORLEVEL 1 ECHO This module is part of PTD.
    ECHO.
    ECHO [1] = Enable, [X] = Remove, [C] = Cancel
    CHOICE /C 1XC /N
    IF !ERRORLEVEL! EQU 1 CALL :EnableModule
    IF !ERRORLEVEL! EQU 2 (
        ECHO Are you sure you want to remove this module?
        CHOICE /C YN
        IF !ERRORLEVEL! EQU 1 CALL :RemoveModule
    )
    IF !ERRORLEVEL! EQU 3 (
        COLOR 0F
        GOTO :ModuleManager
    )
) ELSE (
    COLOR 09
    ECHO Not installed
    CALL :FindModuleFromList !Target!
    IF ERRORLEVEL 1 GOTO :ModuleManager
    ECHO Name: !Name!
    ECHO Author: !Author!
    ECHO Description: !Description!
    ECHO Install method: !InstallType!
    ECHO !InstallFolder! | FINDSTR /C:"PTD" 1>NUL
    IF NOT ERRORLEVEL 1 ECHO This module is part of PTD.
    ECHO.
    ECHO [I] = Install, [C] = Cancel
    CHOICE /C IC /N
    IF !ERRORLEVEL! EQU 1 (
        IF "!InstallType!" EQU "git" CALL :InstallModuleGit
        IF "!InstallType!" EQU "git_r" CALL :InstallModuleGitRecursive
        IF "!InstallType!" EQU "curl_uncompressed" CALL :InstallModuleCurlUncompressed
        IF "!InstallType!" EQU "curl_compressed" CALL :InstallModuleCurlCompressed
        IF "!InstallType!" EQU "git_folder" CALL :InstallModuleGitFolder
    ) ELSE IF !ERRORLEVEL! EQU 2 (
        COLOR 0F
        GOTO :ModuleManager
    )
)
GOTO :ModuleManager

:GetNameOfJSON
SET Counter=0
FOR /r "Syati\Modules" %%f IN (*) DO (
    IF "%%~nxf"=="ModuleInfo.json" (
        IF !Counter! EQU %~1 (
            SET jsonName=%%f
            EXIT /B 1
        )
        SET /A Counter=!Counter!+1
    )
)
FOR /r "Syati\DisabledModules" %%f IN (*) DO (
    IF "%%~nxf"=="ModuleInfo.json" (
        IF !Counter! EQU %~1 (
            SET jsonName=%%f
            EXIT /B 2
        )
        SET /A Counter=!Counter!+1
    )
)
REM Module is installable
EXIT /B 3

:GetModuleName
SET "jsonPath=%~1"
FOR /f "tokens=2,* delims=: " %%a IN ('FINDSTR /C:"\"Name\"" "%jsonPath%"') DO (
    SET Name=%%a %%b
    SET "Name=!Name:"=!"
    SET "Name=!Name:,=!"
    EXIT /B
)

:GetModuleData
SET moduleType=0
SET "jsonPath=%~1"
FOR /f "tokens=2,* delims=: " %%a IN ('FINDSTR /C:"\"Name\"" "!jsonPath!"') DO (
    SET Name=%%a %%b
    SET "Name=!Name:"=!"
    SET "Name=!Name:,=!"
    GOTO :GMD_author
)
:GMD_author
FOR /f "tokens=2,* delims=: " %%a IN ('FINDSTR /C:"\"Author\"" "!jsonPath!"') DO (
    SET Author=%%a %%b
    SET "Author=!Author:"=!"
    SET "Author=!Author:,=!"
    GOTO :GMD_description
)
:GMD_description
FOR /f "tokens=2,* delims=: " %%a IN ('FINDSTR /C:"\"Description\"" "!jsonPath!"') DO (
    SET Description=%%a %%b
    SET "Description=!Description:"=!"
    SET "Description=!Description:,=!"
    GOTO :GMD_dependencies
)
:GMD_dependencies
FOR /f "tokens=2,* delims=: " %%a IN ('FINDSTR /C:"\"ModuleDependancies\"" "!jsonPath!"') DO (
    SET Dependencies=%%a %%b
    SET "Dependencies=!Dependencies:"=!"
    SET "Dependencies=!Dependencies:[=!"
    SET "Dependencies=!Dependencies:]=!"
    FOR %%I IN (!Dependencies!) DO (
        IF EXIST Syati\Modules\%%I (
            ECHO Requires module %%I.
        ) ELSE IF EXIST Syati\DisabledModules\%%I (
            CHOICE /M "This module requires the disabled module %%I. Enable it now" /C YN
            IF !ERRORLEVEL! EQU 1 ROBOCOPY Syati\DisabledModules\%%I Syati\Modules\%%I /E /MOVE >NUL
            IF !ERRORLEVEL! EQU 2 GOTO :ModuleManager
        ) ELSE (
            FINDSTR %%I installable_modules.csv 1>NUL
            IF ERRORLEVEL 1 (
                ECHO Fatal: Module requires unknown module %%I.
                GOTO :ModuleManager
            ) ELSE (
                CHOICE /M "This module requires the downloadable module %%I. Download it now" /C YN
                IF !ERRORLEVEL! EQU 1 (
                    SET OriginalName=!Name!
                    SET Name=%%I
                    FOR /f "tokens=4-5 delims=;" %%J IN ('FINDSTR %%I installable_modules.csv') DO (
                        SET InstallUrl=%%K
                        IF "%%J" EQU "git" CALL :InstallModuleGit
                        IF "%%J" EQU "git_r" CALL :InstallModuleGitRecursive
                        IF "%%J" EQU "curl_uncompressed" CALL :InstallModuleCurlUncompressed
                        IF "%%J" EQU "curl_compressed" CALL :InstallModuleCurlCompressed
                        IF "%%J" EQU "git_folder" CALL :InstallModuleGitFolder
                    )
                    SET Name=!OriginalName!
                )
            )
        )
    )
)
EXIT /B

:GetInstallableModuleData
FOR /f "tokens=1-6 delims=;" %%a IN ('FINDSTR /C:"%~1" installable_modules.csv') DO (
    SET "Name=%%a"
    SET "Author=%%b"
    SET "Description=%%c"
    SET "InstallType=%%d"
    SET "InstallUrl=%%e"
    SET "InstallFolder=%%f"
    EXIT /B
)

:FindModuleFromList
SET Incrementor=0
SET EncodedStr=!ModuleList: =§!
FOR %%I IN (!EncodedStr!) DO (
    SET curName=%%I
    SET curName=!curName:§= !
    IF !Incrementor! EQU %~1 (
        CALL :GetInstallableModuleData "!curName!"
        EXIT /B
    )
    SET /A Incrementor=!Incrementor!+1
)
ECHO FATAL ERROR: ModuleNotFound in FindModuleFromList
EXIT /B 1

:InstallModuleGit
GIT >NUL
IF %ERRORLEVEL% GTR 1 (
    CHOICE /M "Git was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL GIT.GIT
    ECHO.
)
ECHO Cloning module !Name!...
CD Syati/Modules
GIT clone !InstallUrl! 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while cloning module !Name!.
    PAUSE
    GOTO :MainSection
)
FOR %%A IN ("!InstallUrl!") DO (
    SET "part=%%~nxA"
    SET "folderName=!part!"
)
CD ../../
ROBOCOPY Syati\Modules\!folderName!\disc Syati\Output\disc /E >NUL
COLOR 0F
EXIT /B

:InstallModuleGitRecursive
GIT >NUL
IF %ERRORLEVEL% GTR 1 (
    CHOICE /M "Git was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL GIT.GIT
    ECHO.
)
ECHO Cloning module !Name!...
CD Syati/Modules
GIT clone !InstallUrl! --recursive 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while cloning module !Name!.
    PAUSE
    GOTO :MainSection
)
FOR %%A IN ("!InstallUrl!") DO (
    SET "part=%%~nxA"
    SET "folderName=!part!"
)
CD ../../
ROBOCOPY Syati\Modules\!folderName!\disc Syati\Output\disc /E >NUL
COLOR 0F
EXIT /B

:InstallModuleCurlUncompressed
CURL --help >NUL
IF %ERRORLEVEL% GTR 0 (
    CHOICE /M "cURL was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL CURL.CURL
    ECHO.
)
"\Program Files\7-Zip\7z" --help >NUL
IF %ERRORLEVEL% GTR 0 (
    CHOICE /M "7z was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL 7zip.7zip
    ECHO.
)
ECHO Downloading module !Name!...
CD Syati/Modules
CURL -k -L -O !InstallUrl! 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while downloading module !Name!.
    PAUSE
    GOTO :MainSection
)
ECHO Extracting module !Name!...
SET fileName=
FOR %%A IN ("%InstallUrl%") DO (
    SET "part=%%~nxA"
    SET "fileName=!part!"
)
FOR /F "delims=." %%J IN ("!fileName!") DO SET fileNameNoExtension=%%J
"\Program Files\7-Zip\7z" x !fileName! -o!fileNameNoExtension! >NUL
RM -rf !fileName!
CD ../../
ROBOCOPY Syati\Modules\!folderName!\disc Syati\Output\disc /E >NUL
COLOR 0F
EXIT /B

:InstallModuleCurlCompressed
CURL --help >NUL
IF %ERRORLEVEL% GTR 0 (
    CHOICE /M "cURL was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL CURL.CURL
    ECHO.
)
"\Program Files\7-Zip\7z" --help >NUL
IF %ERRORLEVEL% GTR 0 (
    CHOICE /M "7z was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL 7zip.7zip
    ECHO.
)
ECHO Downloading module !Name!...
CD Syati/Modules
CURL -k -L -O !InstallUrl! 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while downloading module !Name!.
    PAUSE
    GOTO :MainSection
)
ECHO Extracting module !Name!...
SET fileName=
FOR %%A IN ("%InstallUrl%") DO (
    SET "part=%%~nxA"
    SET "fileName=!part!"
)
FOR /F "delims=." %%J IN ("!fileName!") DO SET fileNameNoExtension=%%J
"\Program Files\7-Zip\7z" x !fileName! -so | "\Program Files\7-Zip\7z" x -aoa -si -ttar -o!fileNameNoExtension! >NUL
RM -rf !fileName!
CD ../../
ROBOCOPY Syati\Modules\!folderName!\disc Syati\Output\disc /E >NUL
COLOR 0F
EXIT /B

:InstallModuleGitFolder
CURL --help >NUL
IF %ERRORLEVEL% GTR 0 (
    CHOICE /M "cURL was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL CURL.CURL
    ECHO.
)
"\Program Files\7-Zip\7z" --help >NUL
IF %ERRORLEVEL% GTR 0 (
    CHOICE /M "7z was not found. Would you like to download it now" /C YN
    IF %ERRORLEVEL% EQU 2 GOTO :MainSection
    WINGET INSTALL 7zip.7zip
    ECHO.
)
ECHO Downloading module !Name!...
CD Syati/Modules
SET "urlPart=!InstallUrl:~19!"
IF "!urlPart:~-1!" == "\" SET "urlPart=!urlPart:~0,-1!"
FOR /F "tokens=1,2 delims=/" %%A IN ("!urlPart!") DO (
    SET "repoInstallName=%%A/%%B"
)
CURL -k -L -O https://mariogalaxy.org/github2tar?repo=!repoInstallName!^&path=!InstallFolder! 2>NUL
IF %ERRORLEVEL% GTR 1 (
    ECHO Error %ERRORLEVEL% while downloading module !Name!.
    PAUSE
    GOTO :MainSection
)
ECHO Extracting module !Name!...
SET folderComma=!InstallFolder:/=,!
FOR %%J IN (!folderComma!) DO SET folderName=%%J
ECHO !InstallFolder! | FINDSTR /C:"PTD"
IF NOT ERRORLEVEL 1 SET folderName=!folderName!_PTD
"\Program Files\7-Zip\7z" x github2tar -so | "\Program Files\7-Zip\7z" x -aoa -si -ttar -o!folderName! >NUL
RM -rf github2tar
CD ../../
ROBOCOPY Syati\Modules\!folderName!\disc Syati\Output\disc /E >NUL
COLOR 0F
EXIT /B

:DisableModule
SET StartCopyPath=0
SET jsonName=%jsonName:\= %
FOR %%I IN (!jsonName!) DO (
    ECHO %%I | FINDSTR /C:".json" 1>NUL
    IF NOT ERRORLEVEL 1 (
        SET discPath="%CD%\Syati\Modules\!folderName!disc"
        GOTO :DM_AfterLoop
    )
    IF !StartCopyPath! EQU 1 SET folderName=!folderName!%%I\
    IF "%%I" EQU "Modules" SET StartCopyPath=1
)
ECHO FATAL ERROR: ModuleNotFound in RemoveModule.
EXIT /B
:DM_AfterLoop
CD %discPath%
FOR /R %discPath% %%F IN (*) DO (
    SET fullPath=%%F
    SET "relativePath=!fullPath:%CD%=!"
    ECHO %~dp0Syati\Output\disc!relativePath!
    RM -rf "%~dp0Syati\Output\disc!relativePath!"
)
CD %~dp0
ROBOCOPY Syati\Modules\!folderName! Syati\DisabledModules\!folderName! /E /MOVE >NUL
COLOR 0F
EXIT /B

:RemoveModule
SET StartCopyPath=0
SET jsonName=%jsonName: =^ %
SET jsonName=%jsonName:\= %
FOR %%I IN (!jsonName!) DO (
    ECHO %%I | FINDSTR /C:".json" 1>NUL
    IF NOT ERRORLEVEL 1 (
        SET discPath="%CD%\Syati\Modules\!folderName!disc"
        GOTO :RM_AfterLoop
    )
    IF !StartCopyPath! EQU 1 SET folderName=!folderName!%%I\
    IF "%%I" EQU "Modules" SET StartCopyPath=1
)
ECHO FATAL ERROR: ModuleNotFound in RemoveModule.
EXIT /B
:RM_AfterLoop
CD %discPath%
FOR /R %discPath% %%F IN (*) DO (
    SET fullPath=%%F
    SET "relativePath=!fullPath:%CD%=!"
    RM -rf "%~dp0Syati\Output\disc!relativePath!"
)
CD %~dp0
RM -rf Syati\Modules\!folderName! Syati\DisabledModules\!folderName! >NUL
COLOR 0F
EXIT /B

:EnableModule
SET StartCopyPath=0
SET jsonName=%jsonName:\= %
FOR %%I IN (%jsonName%) DO (
    ECHO %%I | FINDSTR /C:".json" 1>NUL
    IF NOT ERRORLEVEL 1 (
        ROBOCOPY Syati\DisabledModules\!folderName! Syati\Modules\!folderName! /E /MOVE >NUL
        ROBOCOPY Syati\Modules\!folderName!\disc Syati\Output\disc /E >NUL
        COLOR 0F
        EXIT /B
    )
    IF !StartCopyPath! EQU 1 SET folderName=!folderName!%%I\
    IF "%%I" EQU "DisabledModules" SET StartCopyPath=1
)
ECHO FATAL ERROR: ModuleNotFound in EnableModule.
EXIT /B

:End
ECHO Abort.
EXIT 0
