using SyatiManager.Source.Common.Helpers;
using System;

namespace SyatiManager.Source.Solutions {
    public class GitFolderInfo : IEquatable<GitFolderInfo> {
        public const string FileName = "GitFolderInfo.json";

        public int Version { get; init; }
        public string[] Files { get; init; } = [];

        public GitFolderInfo() { }

        public GitFolderInfo(string json) {
            JsonHelper.Deserialize(out GitFolderInfo config, json, "The parsed git folder info JSON is null.");

            Version = config.Version;
            Files = config.Files;
        }

        public bool Equals(GitFolderInfo? other) => other is not null && other.Version == Version;
        public override bool Equals(object? obj) => Equals(obj as GitFolderInfo);
        public override int GetHashCode() => Version;
    }
}
