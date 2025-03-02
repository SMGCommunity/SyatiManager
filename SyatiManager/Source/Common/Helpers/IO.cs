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

        public static string GetShortcutPath(string shortcut) {
            try {
                using FileStream fs = File.OpenRead(shortcut);
                using BinaryReader reader = new(fs);
                fs.Seek(0x14, SeekOrigin.Begin);
                var flags = reader.ReadUInt32();

                if ((flags & 1) == 1) {                  // Bit 1 set means we have to skip the shell item ID list
                    fs.Seek(0x4c, SeekOrigin.Begin);     // Seek to the end of the header
                    var offset = reader.ReadUInt16();    // Read the length of the Shell item ID list
                    fs.Seek(offset, SeekOrigin.Current); // Seek past it (to the file locator info)
                }

                var fileInfoStartPos = fs.Position;

                var structLength = reader.ReadUInt32();
                fs.Seek(0xc, SeekOrigin.Current);

                var fileOffset = reader.ReadUInt32();
                fs.Seek(fileInfoStartPos + fileOffset, SeekOrigin.Begin);

                var length = structLength + fileInfoStartPos - fs.Position - 2;
                var link = new string(reader.ReadChars((int)length));

                var begin = link.IndexOf("\0\0");

                if (begin > -1) {
                    var end = link.IndexOf("\\\\", begin + 2) + 2;
                    end = link.IndexOf('\0', end) + 1;

                    var firstPart = link[..begin];
                    var secondPart = link[end..];

                    return firstPart + secondPart;
                }
                return link;
            }
            catch {
                return string.Empty;
            }
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
