using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using LibGit2Sharp;
using SyatiManager.Source.Common;
using SyatiManager.Source.Common.Helpers;
using SyatiManager.Source.Libraries;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyatiManager.Source.Solutions {
    [PseudoClasses(":selected")]
    public partial class ModuleInfo : UserControl {
        public static readonly StyledProperty<string> FolderNameProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(FolderName));

        public static readonly StyledProperty<string> ModuleNameProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(ModuleName));

        public static readonly StyledProperty<string> DescriptionProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(Description));

        public static readonly StyledProperty<string> AuthorProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(Author));

        public static readonly StyledProperty<string[]?> DependenciesProperty =
            AvaloniaProperty.Register<ModuleInfo, string[]?>(nameof(Dependencies));

        public static Solution? Solution {
            get => SyatiCore.Instance.Solution;
        }

        public string FilePath { get; private set; }

        public string FolderPath { get; private set; }

        public string FolderName {
            get => GetValue(FolderNameProperty);
            private set => SetValue(FolderNameProperty, value);
        }

        public string ModuleName {
            get => GetValue(ModuleNameProperty);
            private set => SetValue(ModuleNameProperty, value);
        }

        public string Description {
            get => GetValue(DescriptionProperty);
            private set => SetValue(DescriptionProperty, value);
        }

        public string Author {
            get => GetValue(AuthorProperty);
            private set => SetValue(AuthorProperty, value);
        }

        public string[]? Dependencies {
            get => GetValue(DependenciesProperty);
            private set => SetValue(DependenciesProperty, value);
        }

        public LibraryModule? LibraryModule => SyatiCore.ModuleLibrary[FolderName];

        public InstallSource? Install => LibraryModule?.Install;

        public GitFolderInfo? GitFolderInfo {  get; private set; }

        public ModuleInfo() {
            InitializeComponent();
        }

        public ModuleInfo(string path) : this() {
            JsonHelper.Deserialize(out ModuleInfoData? data, File.ReadAllText(path), $"The parsed module JSON is null. [{path}]");

            FilePath = path;
            FolderPath = Path.GetDirectoryName(path)!;
            FolderName = Path.GetFileName(FolderPath);

            ModuleName = data.Name;
            Description = data.Description;
            Author = data.Author;
            Dependencies = data.InstallDependencies;

            if (data.Description == "[External]") {
                var readme = Path.Combine(FolderPath, "README.md");

                if (File.Exists(readme))
                    Description = File.ReadAllText(readme);
            }

            if (LibraryModule is not null && LibraryModule.Install.IsGitFolder) {
                var gitFolderPath = Path.Combine(FolderPath, "GitFolderInfo.json");

                if (File.Exists(gitFolderPath))
                    GitFolderInfo = new GitFolderInfo(File.ReadAllText(gitFolderPath));
            }
        }

        public void SetSelect(bool value) {
            PseudoClasses.Set(":selected", value);
        }

        public async Task Update() {
            if (LibraryModule is null || Install is null || Solution is null || !await CheckUpdates()) {
                Console.WriteLine($"No updates available for {FolderName}.");
                return;
            }

            try {
                if (Install.Source == InstallSource.SourceType.Git ||
                    Install.Source == InstallSource.SourceType.GitRecursive) {
                    using var repo = new Repository(FolderPath);

                    await repo.MergeAsync(new() {
                        FailOnConflict = true
                    });

                    Solution.AddModule(new ModuleInfo(FilePath), true);
                }
                else {
                    Solution.DeleteModule(this, true);
                    await SyatiCore.Instance.InstallModule(LibraryModule, isUpdate: true);
                }

                Console.WriteLine($"Updated {FolderName} sucessfully.");
            }
            catch (Exception ex) {
                IOHelper.WriteError($"Error while updating {FolderName}", ex);
            }
        }

        public async Task<bool> CheckUpdates() {
            if (Install is null)
                return false;

            try {
                switch (Install.Source) {
                    case InstallSource.SourceType.Git:
                    case InstallSource.SourceType.GitRecursive: {
                            using var repo = new Repository(FolderPath);
                            await repo.FetchAsync("origin");

                            var localCommitTime = repo.Branches["main"].Tip.Committer.When;
                            var remoteCommitTime = repo.Branches["origin/main"].Tip.Committer.When;

                            return localCommitTime < remoteCommitTime;
                        }
                    case InstallSource.SourceType.GitFolder:
                    case InstallSource.SourceType.LGINC: {
                            if (GitFolderInfo is null)
                                return false;

                            using var client = new HttpClient();

                            var rawPath = InstallSource.GetGitFolderRawUrl(Install.ParseGitFolderPathSyntax());
                            var config = new GitFolderInfo(await client.GetStringAsync($"{rawPath}/GitFolderInfo.json"));

                            return GitFolderInfo.Version < config.Version;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while checking for updates", ex);
                return false;
            }
        }

        public void ToggleIgnore() {
            Solution?.ToggleIgnoreEntry(FolderName);
        }

        public void Delete() {
            Solution?.DeleteModule(this);
        }

        public void OpenFolder() {
            Process.Start(new ProcessStartInfo() {
                FileName = FolderPath,
                UseShellExecute = true,
            });
        }
    }

    public class ModuleInfoData {
        public string Name
            { get; set; } = string.Empty;
        public string Description
            { get; set; } = string.Empty;
        public string Author
            { get; set; } = string.Empty;
        public string[]? InstallDependencies
            { get; set; }
    }
}