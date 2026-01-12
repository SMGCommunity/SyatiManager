using Avalonia;
using LibGit2Sharp;
using SyatiManager.Source.Common.Helpers;
using SyatiManager.Source.Libraries;
using SyatiManager.Source.Solutions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SyatiManager.Source.Common {
    public class SyatiCore : AvaloniaObject {
        public static readonly DirectProperty<SyatiCore, Solution?> SolutionProperty =
            AvaloniaProperty.RegisterDirect<SyatiCore, Solution?>(nameof(Solution), o => o.Solution);

        public static readonly DirectProperty<SyatiCore, bool> IsSolutionOpenProperty =
            AvaloniaProperty.RegisterDirect<SyatiCore, bool>(nameof(IsSolutionOpen), o => o.IsSolutionOpen);

        public static readonly DirectProperty<SyatiCore, string> SyatiPathProperty =
            AvaloniaProperty.RegisterDirect<SyatiCore, string>(nameof(SyatiPath), o => o.SyatiPath, (o, v) => o.SyatiPath = v);

        public static readonly DirectProperty<SyatiCore, string> BuildToolFolderProperty =
            AvaloniaProperty.RegisterDirect<SyatiCore, string>(nameof(BuildToolFolder), o => o.BuildToolFolder, (o, v) => o.BuildToolFolder = v);

        public static readonly DirectProperty<SyatiCore, string> CodeWarriorPathProperty =
            AvaloniaProperty.RegisterDirect<SyatiCore, string>(nameof(CodeWarriorPath), o => o.CodeWarriorPath);

        public static readonly DirectProperty<SyatiCore, string> KamekPathProperty =
            AvaloniaProperty.RegisterDirect<SyatiCore, string>(nameof(KamekPath), o => o.KamekPath);

        public static readonly DirectProperty<SyatiCore, bool> AutoUpdateSyatiProperty =
            AvaloniaProperty.RegisterDirect<SyatiCore, bool>(nameof(AutoUpdateSyati), o => o.AutoUpdateSyati, (o, v) => o.AutoUpdateSyati = v);

        public static readonly DirectProperty<SyatiCore, bool> IsUpdatingProperty =
            AvaloniaProperty.RegisterDirect<SyatiCore, bool>(nameof(IsUpdating), o => o.IsUpdating);

        public static readonly string SettingsPath = Path.Combine(AppContext.BaseDirectory, "Settings.json");
        public static readonly ModuleLibrary ModuleLibrary = new(GetComponentPath("Modules.json"));
        public static readonly PresetLibrary PresetLibrary = new(GetComponentPath("Presets.json"));

        private Solution? mSolution;
        private string mSyatiPath;
        private string mBuildToolFolder;
        private bool mAutoUpdateSyati;
        private bool mIsUpdating;

        public Solution? Solution {
            get => mSolution;
            set {
                SetAndRaise(SolutionProperty, ref mSolution, value);
                RaisePropertyChanged(IsSolutionOpenProperty, false, IsSolutionOpen);
            }
        }

        public bool IsSolutionOpen {
            get => mSolution is not null;
        }

        public string SyatiPath {
            get => mSyatiPath;
            set {
                SetAndRaise(SyatiPathProperty, ref mSyatiPath, value);
                RaisePropertyChanged(CodeWarriorPathProperty, string.Empty, CodeWarriorPath);
                RaisePropertyChanged(KamekPathProperty, string.Empty, KamekPath);
            }
        }

        public string BuildToolFolder {
            get => mBuildToolFolder;
            set => SetAndRaise(BuildToolFolderProperty, ref mBuildToolFolder, value);
        }

        public bool IsSyatiInstalled {
            get => !string.IsNullOrEmpty(mSyatiPath)
                && Directory.Exists(mSyatiPath)
                && Repository.IsValid(mSyatiPath);
        }

        public bool AutoUpdateSyati {
            get => mAutoUpdateSyati;
            set => SetAndRaise(AutoUpdateSyatiProperty, ref mAutoUpdateSyati, value);
        }

        public bool IsUpdating {
            get => mIsUpdating;
            set => SetAndRaise(IsUpdatingProperty, ref mIsUpdating, value);
        }

        private static readonly string CodeWarriorExeName = OperatingSystem.IsWindows() ? "mwcceppc.exe" : "mwcceppc";
        private static readonly string KamekExeName = OperatingSystem.IsWindows() ? "Kamek.exe" : "Kamek";
        private static readonly string BuildToolName = OperatingSystem.IsWindows() ? "SyatiModuleBuildTool.exe" : "SyatiModuleBuildTool";

        public string CodeWarriorFolder => Path.Combine(mSyatiPath, "deps", "CodeWarrior");
        public string CodeWarriorPath => Path.Combine(CodeWarriorFolder, CodeWarriorExeName);
        public string KamekFolder => Path.Combine(mSyatiPath, "deps", "Kamek");
        public string KamekPath => Path.Combine(KamekFolder, KamekExeName);
        public string BuildToolPath => Path.Combine(BuildToolFolder, BuildToolName);

        private SyatiCore() {
            mSyatiPath = string.Empty;
            mBuildToolFolder = string.Empty;
        }

        public void LoadSolution(string path) {
            Solution?.Save();

            try {
                Solution = new Solution(path);
                Console.WriteLine($"Loaded solution successfully! [{Solution.Modules.Count} modules]");
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while opening solution", ex);
            }
        }

        public void SaveSolution() {
            if (mSolution is null)
                return;

            try {
                mSolution.Save();
                Console.WriteLine("Saved solution successfully.");
            }
            catch (Exception ex) {
                IOHelper.WriteError($"Error while saving solution", ex);
            }
        }

        public async Task BuildCode() {
            if (!ValidateBuilding())
                return;

            var regions = mSolution.Regions.ToList();
            if (regions.Count == 0) {
                Console.WriteLine("No regions specified.");
                return;
            }

            var unibuild = mSolution.Unibuild switch {
                UnibuildType.Always => true,
                UnibuildType.Auto => mSolution.Modules.Count > 10,
                _ => false
            };

            Console.WriteLine($"\nBuilding code [{string.Join(", ", regions)} | {DateTime.Now}]");
            mSolution.Save();
            mSolution.CreateDirectories();

            foreach (var region in regions) {
                try {
                    var regionName = region switch {
                        "USA" => "SB4E",
                        "PAL" => "SB4P",
                        "JPN" => "SB4J",
                        "TWN" => "SB4W",
                        "KOR" => "SB4K",
                        _ => string.Empty
                    };

                    await IOHelper.StartProcessAsync(BuildToolPath, [
                        regionName, mSyatiPath, mSolution.ModulesPath, mSolution.OutputPath, (unibuild ? "-u" : string.Empty)
                    ]);
                }
                catch (Exception ex) {
                    IOHelper.WriteError("SyatiModuleBuildTool Error", ex);
                    return;
                }
            }

            Console.WriteLine($"Running build tasks...");

            try {
                mSolution.RunAllTasks();
            }
            catch (Exception ex) {
                IOHelper.WriteError("Build Task Error", ex);
                return;
            }

            Console.WriteLine($"Code built successfully [{DateTime.Now}]");
        }

        public async Task BuildLoader() {
            if (!ValidateBuilding())
                return;

            var regions = mSolution.Regions.ToList();
            if (regions.Count == 0) {
                Console.WriteLine("No regions specified.");
                return;
            }

            Console.WriteLine($"\nBuilding loader [{string.Join(", ", regions)} | {DateTime.Now}]");
            mSolution.CreateDirectories();

            foreach (var region in regions) {
                var regionName = region switch {
                    "USA" => "SB4E",
                    "PAL" => "SB4P",
                    "JPN" => "SB4J",
                    "TWN" => "SB4W",
                    "KOR" => "SB4K",
                    _ => string.Empty
                };

                var loaderObj = Path.Combine(mSyatiPath, "loader", "loader.o");

                try {
                    await IOHelper.StartProcessAsync(CodeWarriorPath, [
                        "-c", "-Cpp_exceptions", "off", "-nodefaults", "-proc", "gekko", "-fp",
                        "hard", "-lang=c++", "-O4,s", "-inline", "on", "-rtti", "off", "-sdata", "0",
                        "-sdata2", "0", "-align", "powerpc", "-func_align", "4", "-str", "pool",
                        "-enum", "int", "-DGEKKO", "-i", "include", "-I-", "-i", "loader", $"-D{regionName}",
                        Path.Combine(mSyatiPath, "loader", "loader.cpp"), "-o", loaderObj
                    ],
                    mSyatiPath);
                }
                catch (Exception ex) {
                    IOHelper.WriteError("CodeWarrior Error", ex);
                    return;
                }

                try {
                    await IOHelper.StartProcessAsync(KamekPath, [
                        loaderObj, "-static=0x80001800", $"-externals={Path.Combine(mSyatiPath, "symbols", $"{regionName}.txt")}",
                        $"-output-riiv={Path.Combine(mSolution.OutputPath, $"riivo_{regionName}.xml")}",
                        $"-output-code={Path.Combine(mSolution.OutputPath, $"Loader{regionName}.bin")}"
                    ],
                    mSyatiPath);
                }
                catch (Exception ex) {
                    IOHelper.WriteError("Kamek Error", ex);
                    return;
                }
            }

            Console.WriteLine($"Loader built successfully [{DateTime.Now}]");
        }

        [MemberNotNullWhen(true, nameof(mSolution), nameof(mSyatiPath), nameof(BuildToolPath))]
        private bool ValidateBuilding() {
            if (mSolution is null)
                return false;

            if (string.IsNullOrEmpty(mSyatiPath) || !Directory.Exists(mSyatiPath)) {
                Console.WriteLine("Syati path does not exist, set it in the Build Settings menu.");
                return false;
            }

            if (string.IsNullOrEmpty(BuildToolPath) || !File.Exists(BuildToolPath)) {
                Console.WriteLine("SyatiModuleBuildTool path does not exist, set it in the Build Settings menu.");
                return false;
            }

            mSolution.Save();
            return true;
        }

        public void GenVSCodeConfiguration() {
            if (mSolution is null)
                return;

            try {
                mSolution.CreateDirectories();

                Directory.CreateDirectory(mSolution.VSCodeFolder);
                File.Copy(GetComponentPath("VSC.Configuration.json"), Path.Combine(mSolution.VSCodeFolder, "c_cpp_properties.json"), true);

                Console.WriteLine("Generated VSCode C++ configuration sucessfully.");
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while generating VSCode C++ configuration", ex);
            }
        }

        public void GenVSCodeBuildTask() {
            if (mSolution is null)
                return;

            try {
                var content = File.ReadAllText(GetComponentPath("VSC.BuildTask.json"));
                content = content.Replace("{{ReplaceCommandTarget}}", $"\\\"{Path.Combine(AppContext.BaseDirectory, "SyatiManager.exe").Replace('\\', '/')}\\\" \\\"{mSolution.FilePath}\\\" -b --no-gui");

                Directory.CreateDirectory(mSolution.VSCodeFolder);
                File.WriteAllText(Path.Combine(mSolution.VSCodeFolder, "tasks.json"), content);

                Console.WriteLine("Generated VSCode build task sucessfully.");
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while generating VSCode C++ configuration", ex);
            }
        }

        public async Task UpdateAllComponents() {
            if (mIsUpdating)
                return;

            IsUpdating = true;
            await UpdateSyati();
            await InstallCodeWarrior();
            await InstallKamek();
            await UpdateBuildTool();
            IsUpdating = false;
        }

        public async Task UpdateSyati() {
            if (string.IsNullOrEmpty(mSyatiPath)) {
                Console.WriteLine("Syati Path is undefined, set it in the Build Settings menu.");
                return;
            }

            if (!Directory.Exists(mSyatiPath) || !Repository.IsValid(mSyatiPath)) {
                try {
                    await IOHelper.CloneAsync("https://github.com/SMGCommunity/Syati/", mSyatiPath, new() { RecurseSubmodules = true });
                    Console.WriteLine("Installed Syati successfully.");
                }
                catch (Exception ex) {
                    IOHelper.WriteError("Error while installing Syati", ex);
                }
                
                return;
            }

            try {
                using var repo = new Repository(mSyatiPath);

                await repo.FetchAsync("origin");
                await repo.MergeAsync(new() { FailOnConflict = true });

                Console.WriteLine("Updated Syati successfully.");
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while updating Syati", ex);
            }
        }

        public async Task InstallCodeWarrior() {
            if (Directory.Exists(CodeWarriorFolder))
                return;

            if (!IsSyatiInstalled) {
                Console.WriteLine("Cannot install Code Warrior, Syati is not installed.");
                return;
            }

            try {
                using var client = new HttpClient();
                if (OperatingSystem.IsWindows()) {
                    using var stream = await client.GetStreamAsync("https://mariogalaxy.org/CodeWarrior-Syati.zip");
                    ZipFile.ExtractToDirectory(stream, CodeWarriorFolder, true);
                } else {
                    if (!Directory.Exists(CodeWarriorFolder)) 
                        Directory.CreateDirectory(CodeWarriorFolder);

                    using var compilerStream = await client.GetStreamAsync("https://mariogalaxy.org/mwcceppc-syati");
                    using var compilerDest = File.OpenWrite(Path.Combine(CodeWarriorFolder, "mwcceppc"));
                    compilerStream.CopyTo(compilerDest);

                    using var assemblerStream = await client.GetStreamAsync("https://mariogalaxy.org/mwasmeppc-syati");
                    using var assemblerDest = File.OpenWrite(Path.Combine(CodeWarriorFolder, "mwasmeppc"));
                    assemblerStream.CopyTo(assemblerDest);

                    Process.Start($"chmod +x {Path.Combine(CodeWarriorFolder, "mwcceppc")}");
                    Process.Start($"chmod +x {Path.Combine(CodeWarriorFolder, "mwasmeppc")}");
                }
                Console.WriteLine("Installed CodeWarrior successfully.");
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while installing CodeWarrior", ex);
            }
        }

        public async Task InstallKamek() {
            if (Directory.Exists(KamekFolder))
                return;

            if (!IsSyatiInstalled) {
                Console.WriteLine("Cannot install Kamek, Syati is not installed.");
                return;
            }

            try {
                using var client = new HttpClient();

                if (OperatingSystem.IsWindows()) {
                    using var stream = await client.GetStreamAsync("https://github.com/Treeki/Kamek/releases/download/2024-04-10_prerelease/kamek_2024-04-10_win-x64.zip");
                    ZipFile.ExtractToDirectory(stream, KamekFolder, true);
                }
                else {
                    string url = OperatingSystem.IsMacOS() ?
                        "https://github.com/Treeki/Kamek/releases/download/2024-04-10_prerelease/kamek_2024-04-10_mac-x64.tar.gz" :
                        "https://github.com/Treeki/Kamek/releases/download/2024-04-10_prerelease/kamek_2024-04-10_linux-x64.tar.gz";

                    using var stream = await client.GetStreamAsync(url);
                    await IOHelper.ExtractTarGz(stream, KamekFolder);
                }

                Console.WriteLine("Installed Kamek successfully.");
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while installing Kamek", ex);
            }
        }

        public async Task UpdateBuildTool() {
            if (File.Exists(BuildToolPath))
                return;

            var url =
                OperatingSystem.IsWindows() ? "https://github.com/SMGCommunity/SyatiModuleBuildTool/releases/download/auto/SyatiModuleBuildTool.exe" :
                OperatingSystem.IsMacOS() ? "https://github.com/SMGCommunity/SyatiModuleBuildTool/releases/download/auto/SyatiModuleBuildTool-macos" :
                "https://github.com/SMGCommunity/SyatiModuleBuildTool/releases/download/auto/SyatiModuleBuildTool-linux";

            try {
                Directory.CreateDirectory(mBuildToolFolder);

                using var client = new HttpClient();
                using var stream = await client.GetStreamAsync(url);
                using var fs = File.OpenWrite(BuildToolPath);

                await stream.CopyToAsync(fs);
                Console.WriteLine("Installed SyatiModuleBuildTool successfully.");
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while installing SyatiModuleBuildTool", ex);
            }
        }

        public void ReloadModules() {
            if (mSolution is not null)
                mSolution.ModulesPath = mSolution.ModulesPath;
        }

        public async Task InstallModule(LibraryModule libModule, bool isDependency = false, bool isUpdate = false, Solution? solution = null) {
            solution ??= mSolution;
            if (solution is null)
                return;
            
            if (!isUpdate && solution.IsModuleInstalled(libModule.FolderName)) {
                Console.WriteLine($"{libModule.FolderName} is already installed.");
                return;
            }

            try {
                Console.WriteLine($"{(isUpdate ? "Updating" : "Installing")} module {libModule.FolderName}...");

                var folder = Path.Combine(solution.ModulesPath, libModule.FolderName);
                var infoPath = Path.Combine(folder, "ModuleInfo.json");

                await libModule.Install.InstallAsync(folder);

                var module = new ModuleInfo(infoPath);
                solution.AddModule(module, true);

                Console.WriteLine($"{(isUpdate ? "Updated" : "Installed")} {libModule.FolderName} successfully.");

                if (!isDependency)
                    Console.Write("\x1b[2;99m");

                if (module.Dependencies is not null) {
                    foreach (var dependency in module.Dependencies) {
                        var dependencyModule = ModuleLibrary[dependency];

                        if (dependencyModule is not null)
                            await InstallModule(dependencyModule, true);
                    }
                }

                if (!isDependency) {
                    Console.Write("\x1B[0m");
                    Console.WriteLine($"Installed all {libModule.FolderName} dependencies.");
                }
            }
            catch (Exception ex) {
                IOHelper.WriteError($"Error while installing {libModule.FolderName}", ex);
                return;
            }
        }

        public async Task InstallPreset(ModulePreset preset, PresetInfo info) {
            if (mSolution is null)
                return;

            try {
                Console.WriteLine($"Installing preset \"{preset.PresetName}\".");

                var folder = Path.Combine(mSolution.ModulesPath, info.FolderName);
                var infoPath = Path.Combine(folder, "ModuleInfo.json");

                await preset.Install.InstallAsync(folder);

                foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)) {
                    try {
                        var relativePath = Path.GetRelativePath(folder, file);
                        var newRelativePath = ApplyReplace(relativePath);

                        if (relativePath != newRelativePath) {
                            await File.WriteAllTextAsync(Path.Combine(folder, newRelativePath), ApplyReplace(await File.ReadAllTextAsync(file)));
                            File.Delete(file);

                            continue;
                        }

                        await File.WriteAllTextAsync(file, ApplyReplace(await File.ReadAllTextAsync(file)));
                    }
                    catch (Exception ex) {
                        IOHelper.WriteError($"Error while applying preset to \"{file}\"", ex);
                    }
                }

                var module = new ModuleInfo(infoPath);
                mSolution.AddModule(module, true);

                if (module.Dependencies is not null) {
                    foreach (var dependency in module.Dependencies) {
                        var dependencyModule = ModuleLibrary[dependency];

                        if (dependencyModule is not null)
                            await InstallModule(dependencyModule, true);
                    }
                }
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while installing preset", ex);
            }

            string ApplyReplace(string s) {
                return s
                    .Replace("PresetTarget_InternalName", info.FolderName)
                    .Replace("PresetTarget_Name", info.Name)
                    .Replace("PresetTarget_Author", info.Author)
                    .Replace("PresetTarget_Description", info.Description);
            }
        }

        public void LoadSettings() {
            if (!File.Exists(SettingsPath))
                return;

            var settings = new AppSettings(File.ReadAllText(SettingsPath));

            SyatiPath = settings.SyatiPath;
            BuildToolFolder = settings.BuildToolFolder;
            AutoUpdateSyati = settings.AutoUpdateSyati;
        }

        public void SaveSettings() {
            var settings = new AppSettings {
                SyatiPath = mSyatiPath,
                BuildToolFolder = mBuildToolFolder,
                AutoUpdateSyati = mAutoUpdateSyati,
            };

            settings.Save(SettingsPath);
        }

        public static string GetComponentPath(string compName) {
            return Path.Combine(AppContext.BaseDirectory, "Components", compName);
        }

        public static readonly SyatiCore Instance = new();
    }

    public class AppSettings {
        public string SyatiPath { get; set; }
        public string BuildToolFolder { get; set; }
        public bool AutoUpdateSyati { get; set; }

        public AppSettings() {
            SyatiPath = string.Empty;
            BuildToolFolder = string.Empty;
        }

        public AppSettings(string json) {
            JsonHelper.Deserialize(out AppSettings settings, json, "The parsed git folder info JSON is null.");

            SyatiPath = settings.SyatiPath ?? string.Empty;
            BuildToolFolder = settings.BuildToolFolder ?? string.Empty;
            AutoUpdateSyati = settings.AutoUpdateSyati;
        }

        public void Save(string path) {
            File.WriteAllText(path, JsonHelper.Serialize(this));
        }
    }
}
