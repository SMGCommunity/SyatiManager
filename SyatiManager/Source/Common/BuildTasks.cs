using SyatiManager.Source.Solutions;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SyatiManager.Source.Common {
    public enum BuildTaskType {
        Copy,
        Command,
        Arc_Append,
        Bcsv_Append,
        Bcsv_Write
    }

    public class BuildTask {
        public BuildTaskType Type { get; set; }

        public virtual void Run(Solution sln) { }

        public static BuildTask? Deserialize(JsonNode node, JsonSerializerOptions? options) {
            var task = node.Deserialize<BuildTask>(options);

            if (task is not null) {
                return task.Type switch {
                    BuildTaskType.Copy => node.Deserialize<CopyTask>(options),
                    BuildTaskType.Command => node.Deserialize<CommandTask>(options),
                    BuildTaskType.Arc_Append => node.Deserialize<ArcAppendTask>(options),
                    BuildTaskType.Bcsv_Append => node.Deserialize<BcsvAppendTask>(options),
                    BuildTaskType.Bcsv_Write => node.Deserialize<BcsvWriteTask>(options),
                    _ => throw new JsonException($"Unknown BuildTask: {task.Type}")
                };
            }
            
            return task;
        }
    }

    public class CopyTask : BuildTask {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public bool Recurse { get; set; }

        public override void Run(Solution sln) {
            foreach (var file in Directory.EnumerateDirectories(Path.GetDirectoryName(Source)!, Path.GetFileName(Source)!, Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                File.Copy(file, Path.Join(Target, file), true);
        }
    }

    public class CommandTask : BuildTask {
        public string Windows { get; set; } = string.Empty;
        public string Linux { get; set; } = string.Empty;
        public string MacOS { get; set; } = string.Empty;

        public override void Run(Solution sln) {
            Process proc;

            if (OperatingSystem.IsWindows())
                proc = Process.Start(Windows);
            else if (OperatingSystem.IsMacOS())
                proc = Process.Start(MacOS);
            else
                proc = Process.Start(Linux);

            proc.WaitForExit();
        }
    }

    public class ArcAppendTask : BuildTask {
        public string RequestArc { get; set; } = string.Empty;
        public string ApendFilePath { get; set; } = string.Empty;
    }

    public record AppendRow(string ModelName, string ClassName);

    public class BcsvAppendTask : BuildTask {
        public string RequestArc { get; set; } = string.Empty;
        public string FromBcsv { get; set; } = string.Empty;
        public AppendRow[] AppendRows { get; set; } = [];
    }

    public class BcsvWriteTask : BuildTask {
        public string RequestArc { get; set; } = string.Empty;
        public string FromBcsv { get; set; } = string.Empty;
        public AppendRow[] AppendRows { get; set; } = [];
    }
}