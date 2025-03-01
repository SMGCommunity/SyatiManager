using Avalonia;
using Avalonia.Collections;
using SyatiManager.Source.Common;
using SyatiManager.Source.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SyatiManager.Source.Solutions {
    public partial class Solution : AvaloniaObject {
        private readonly SelectionHolder<ModuleInfo> mSelectionHolder;
        private AvaloniaList<ModuleInfo> mModules;
        private AvaloniaList<IgnoreEntry> mIgnoreEntries;

        private readonly string mFilePath;
        private readonly string mFolderPath;
        private string mModulesPath;
        private string mOutputPath;
        private Regions mRegions;
        private UnibuildType mUnibuild;
        private List<BuildTask> mTasks;

        public string FilePath {
            get => mFilePath;
        }

        public string FolderPath {
            get => mFolderPath;
        }

        public string VSCodeFolder {
            get => Path.Combine(ModulesPath, ".vscode");
        }

        public string IgnorePath {
            get => Path.Combine(ModulesPath, ".moduleignore");
        }

        public string RelativeModulesPath {
            get => mModulesPath;
            set {
                SetAndRaise(ModulesPathProperty, ref mModulesPath, value);
                LoadModules();
            }
        }

        public string RelativeOutputPath {
            get => mOutputPath;
            set => SetAndRaise(OutputPathProperty, ref mOutputPath, value);
        }

        public string ModulesPath {
            get => Path.GetFullPath(Path.Combine(mFolderPath, mModulesPath));
            set {
                SetAndRaise(ModulesPathProperty, ref mModulesPath, Path.GetRelativePath(mFolderPath, value));
                LoadModules();
            }
        }

        public string OutputPath {
            get => Path.GetFullPath(Path.Combine(mFolderPath, mOutputPath));
            set => SetAndRaise(OutputPathProperty, ref mOutputPath, Path.GetRelativePath(mFolderPath, value));
        }

        public UnibuildType Unibuild {
            get => mUnibuild;
            set => mUnibuild = value;
        }

        public Regions Regions {
            get => mRegions;
            set => mRegions = value;
        }

        public AvaloniaList<ModuleInfo> Modules {
            get => mModules;
        }

        public AvaloniaList<IgnoreEntry> IgnoreEntries {
            get => mIgnoreEntries;
        }

        public List<BuildTask> Tasks {
            get => mTasks;
        }

        public ModuleInfo? SelectedModule {
            get => mSelectionHolder.SelectedItem;
        }

        public Solution(string path) {
            mSelectionHolder = new();
            mModules = [];
            mIgnoreEntries = [];
            mTasks = [];

            mFilePath = path;
            mFolderPath = Path.GetDirectoryName(path)!;

            if (File.Exists(path)) {
                LoadConfig();
            }
            else {
                mModulesPath = "Modules/";
                mOutputPath = "Output/";
                CreateDirectories();
                Save();
            }

            LoadModules();
            LoadIgnored();
        }

        private void LoadConfig() {
            JsonHelper.Deserialize(out SolutionData data, File.ReadAllText(mFilePath), $"Parsed solution JSON is null. [{mFilePath}]");

            mModulesPath = data.ModulesPath;
            mOutputPath = data.OutputPath;
            mRegions = data.Regions;
            mUnibuild = data.Unibuild;
            mTasks = data.Tasks;
        }

        private void LoadModules() {
            mModules.Clear();

            foreach (var directory in Directory.EnumerateDirectories(ModulesPath, "*", SearchOption.TopDirectoryOnly)) {
                var infoPath = Path.Combine(directory, "ModuleInfo.json");

                if (!File.Exists(infoPath))
                    continue;

                try {
                    AddModule(new(infoPath));
                }
                catch (Exception ex) {
                    IOHelper.WriteError("Error while loading module", ex);
                }
            }
        }

        private void LoadIgnored() {
            if (!File.Exists(IgnorePath))
                return;

            foreach (var line in File.ReadLines(IgnorePath)) {
                mIgnoreEntries.Add(new(line));
            }
        }

        public void Save() {
            var data = new SolutionData() {
                ModulesPath = mModulesPath,
                OutputPath = mOutputPath,
                Regions = mRegions,
                Unibuild = mUnibuild,
                Tasks = mTasks
            };

            File.WriteAllText(mFilePath, JsonHelper.Serialize(in data));

            var sb = new StringBuilder();

            foreach (var ignore in mIgnoreEntries)
                sb.AppendLine(ignore.FolderName);

            File.WriteAllText(IgnorePath, sb.ToString());
        }

        public bool GetRegion(Regions region) {
            return (Regions & region) != 0;
        }

        public void SetRegion(Regions region, bool value) {
            if (value)
                Regions |= region;
            else
                Regions &= ~region;
        }

        public void CreateDirectories() {
            Directory.CreateDirectory(ModulesPath);
            Directory.CreateDirectory(OutputPath);
        }

        public void AddModule(ModuleInfo module, bool overrideExisting = false) {
            if (overrideExisting)
                RemoveModule(GetModule(module.FolderName));

            module.PointerPressed += ModuleClicked;
            mModules.Add(module);
        }

        public void RemoveModule(ModuleInfo? module) {
            if (module is not null)
                mModules.Remove(module);
        }

        public void DeleteModule(ModuleInfo module, bool isUpdate = false) {
            if (!isUpdate)
                Console.WriteLine($"Deleting module {module.FolderName}.");

            if (Directory.Exists(module.FolderPath)) {
                var gitFolder = Path.Combine(module.FolderPath, ".git");

                if (Directory.Exists(gitFolder)) {
                    foreach (var file in Directory.EnumerateFiles(gitFolder, "*", SearchOption.AllDirectories)) {
                        File.SetAttributes(file, FileAttributes.Normal);
                    }
                }

                Directory.Delete(module.FolderPath, true);
            }


            if (!isUpdate)
                mModules.Remove(module);

            if (mSelectionHolder.SelectedItem == module)
                SelectModule(null);
        }

        public void AddIgnoreEntry(string folderName) {
            Console.WriteLine($"Added ignore entry for folder \"{folderName}\".");
            mIgnoreEntries.Add(new(folderName));
        }

        public void RemoveIgnoreEntry(IgnoreEntry entry) {
            Console.WriteLine($"Removed ignore entry for folder \"{entry.FolderName}\".");
            mIgnoreEntries.Remove(entry);
        }

        public void ToggleIgnoreEntry(string folderName) {
            var entry = GetIgnoreEntry(folderName);

            if (entry is not null)
                RemoveIgnoreEntry(entry);
            else
                AddIgnoreEntry(folderName);
        }

        public IgnoreEntry? GetIgnoreEntry(string folderName) {
            return mIgnoreEntries.FirstOrDefault(x => x.FolderName.Equals(folderName, StringComparison.CurrentCultureIgnoreCase));
        }

        public void RunAllTasks() {
            foreach (var task in mTasks) {
                task.Run(this);
            }
        }

        public void SelectModule(ModuleInfo? module) {
            SelectedModule?.SetSelect(false);
            mSelectionHolder.Select(module);
            module?.SetSelect(true);

            RaisePropertyChanged(SelectedModuleProperty, null, module);
        }

        public ModuleInfo? GetModule(string folderName) {
            return mModules.FirstOrDefault(m => m.FolderName.Equals(folderName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsModuleInstalled(string folderName) {
            return GetModule(folderName) is not null;
        }

        public bool IsModuleIgnored(string folderName) {
            return GetIgnoreEntry(folderName) is not null;
        }
    }

    public class SolutionData {
        public string ModulesPath { get; set; } = string.Empty;

        public string OutputPath { get; set; } = string.Empty;

        public Regions Regions { get; set; }

        public UnibuildType Unibuild { get; set; }

        public List<BuildTask> Tasks { get; set; } = [];
    }
}
