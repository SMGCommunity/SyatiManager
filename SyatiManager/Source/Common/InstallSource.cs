using LibGit2Sharp;
using SyatiManager.Source.Common.Helpers;
using SyatiManager.Source.Solutions;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyatiManager.Source.Common {
    public partial class InstallSource {
        private readonly SourceType mSource;
        private readonly string mUrl;

        public SourceType Source {
            get => mSource;
            init => mSource = value;
        }

        public string Url {
            get => mSource == SourceType.LGINC ? $"https://github.com/Lord-G-INC/Modular-PTD/tree/main/PTD/{mUrl}" : mUrl;
            init => mUrl = value;
        }

        public bool IsGitFolder => mSource == SourceType.GitFolder || mSource == SourceType.LGINC;

        public InstallSource(SourceType type, string path) {
            mSource = type;
            mUrl = path;
        }

        public InstallSource(string str) {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException(nameof(str), "InstallSource string is empty.");

            var parts = str.Split(':', 2);

            if (parts.Length != 2)
                throw new FormatException("Invalid InstallSource formatting.");

            mSource = StringToSourceType(parts[0]);
            mUrl = parts[1];
        }

        private static SourceType StringToSourceType(string str) => str switch {
            "git" => SourceType.Git,
            "git_recursive" => SourceType.GitRecursive,
            "git_folder" => SourceType.GitFolder,
            "lginc" => SourceType.LGINC,
            _ => throw new FormatException($"Unknown SourceType \"{str}\".")
        };

        private static string SourceTypeToString(SourceType type) => type switch {
            SourceType.Git => "git",
            SourceType.GitRecursive => "git_recursive",
            SourceType.GitFolder => "git_folder",
            SourceType.LGINC => "lginc",
            _ => throw new FormatException($"Unknown SourceType \"{type}\".")
        };

        public async Task InstallAsync(string path) {
            Directory.CreateDirectory(path);

            switch (mSource) {
                case SourceType.Git:
                    DownloadGit(path);
                    break;
                case SourceType.GitRecursive:
                    DownloadGitRecursive(path);
                    break;
                case SourceType.GitFolder:
                case SourceType.LGINC:
                    await DownloadGitFolder(path);
                    break;
            }
        }

        private void DownloadGit(string path) {
            Repository.Clone(mUrl, path);
        }

        private void DownloadGitRecursive(string path) {
            Repository.Clone(mUrl, path, new() {
                RecurseSubmodules = true
            });
        }

        private async Task DownloadGitFolder(string path) {
            using var client = new HttpClient();

            var rawPath = GetGitFolderRawUrl(ParseGitFolderPathSyntax());
            var configContent = await client.GetStringAsync($"{rawPath}/GitFolderInfo.json");
            var config = new GitFolderInfo(configContent);

            using (var moduleInfoMsg = await client.SendAsync(new(HttpMethod.Get, $"{rawPath}/ModuleInfo.json"))) {
                if (moduleInfoMsg.IsSuccessStatusCode) {
                    using var stream = await moduleInfoMsg.Content.ReadAsStreamAsync();
                    using var fs = File.OpenWrite(Path.Combine(path, "ModuleInfo.json"));
                    await stream.CopyToAsync(fs);
                }
            }

            File.WriteAllText(Path.Combine(path, "GitFolderInfo.json"), configContent);

            foreach (var file in config.Files) {
                var filePath = Path.Combine(path, file);
                var dir = Path.GetDirectoryName(filePath);

                if (dir is not null)
                    Directory.CreateDirectory(dir);

                try {
                    using var stream = await client.GetStreamAsync($"{rawPath}/{file}");
                    using var fs = File.OpenWrite(filePath);
                    await stream.CopyToAsync(fs);
                }
                catch (Exception ex) {
                    IOHelper.WriteError($"Unable to download \"{rawPath}/{file}\"", ex);
                }
            }
        }

        public override string ToString()
            => string.Concat(SourceTypeToString(mSource), ":", mUrl);

        public static implicit operator string(InstallSource s) => s.ToString();
        public static implicit operator InstallSource(string s) => new(s);

        public enum SourceType {
            Git,
            GitRecursive,
            GitFolder,
            LGINC
        }

        public readonly struct GitFolderPathSyntax {
            public string Repo { get; init; }
            public string Tree { get; init; }
            public string Folder { get; init; }
        }

        public GitFolderPathSyntax ParseGitFolderPathSyntax() {
            var match = GitFolderRegex().Match(Url);

            if (!match.Success) {
                throw new FormatException("Git Folder syntax is invalid.");
            }

            return new() {
                Repo = match.Groups["Repo"].Value,
                Tree = match.Groups["Tree"].Value,
                Folder = match.Groups["Folder"].Value
            };
        }

        public static string GetGitFolderRawUrl(in GitFolderPathSyntax syntax) {
            return $"https://raw.githubusercontent.com/{syntax.Repo}/refs/heads/{syntax.Tree}/{syntax.Folder}";
        }

        [GeneratedRegex(@"https:\/\/github\.com\/(?<Repo>[^\/ ]+\/[^\/ ]+)\/tree\/(?<Tree>[^\/ ]+)\/(?<Folder>[^ ]+)")]
        private static partial Regex GitFolderRegex();
    }
}
