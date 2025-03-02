using LibGit2Sharp;
using System;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SyatiManager.Source.Common.Helpers {
    public static class IOHelper {
        public static async Task StartProcessAsync(string fileName, IReadOnlyList<string> args, string? workdir = null) {
            var info = new ProcessStartInfo() {
                FileName = fileName
            };

            foreach (var arg in args)
                info.ArgumentList.Add(arg);

            if (workdir is not null)
                info.WorkingDirectory = workdir;

            using var proc = Process.Start(info) ?? throw new NullReferenceException($"Process is null.");
            await proc.WaitForExitAsync();

            if (proc.ExitCode != 0)
                throw new Exception($"Process exited with code {proc.ExitCode}.");
        }

        public static async Task ExtractTarGz(Stream source, string outFolder) {
            Directory.CreateDirectory(outFolder);

            using var decompressor = new GZipStream(source, CompressionMode.Decompress);
            await TarFile.ExtractToDirectoryAsync(decompressor, outFolder, true);
        }

        public static async Task<string> CloneAsync(string url, string path, CloneOptions options) {
            return await Task.Run(() => Repository.Clone(url, path, options));
        }

        public static async Task FetchAsync(this Repository repo, string url) {
            await Task.Run(() => repo.Network.Fetch(url, []));
        }

        public static async Task<MergeResult> MergeAsync(this Repository repo, MergeOptions options) {
            return await Task.Run(() => repo.MergeFetchedRefs(repo.Config.BuildSignature(DateTimeOffset.Now), options));
        }

        public static void WriteError(string msg, Exception ex) {
            Console.WriteLine(string.Concat(
                $"\x1B[0;31m", msg, ": ", ex.Message, "\x1B[0m"
#if DEBUG
                , $"\n{ex.StackTrace}"
#endif
                ));
        }
    }
}
