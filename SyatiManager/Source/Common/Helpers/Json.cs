using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SyatiManager.Source.Common.Helpers {
    public static class JsonHelper {
        public static readonly JsonSerializerOptions DefaultOptions = new() {
            WriteIndented = true,
            Converters = {
                new InstallSourceJsonConverter(),
                new UnibuildTypeJsonConverter(),
                new RegionsJsonConverter(),
                new BuildTaskTypeConverter(),
                new BuildTaskListConverter()
            }
        };

        public static void Deserialize<T>([NotNullIfNotNull(nameof(throwMessage))] out T? target, string json, string? throwMessage = null) {
            target = JsonSerializer.Deserialize<T>(json, DefaultOptions);

            if (target is null && throwMessage is not null) {
                throw new JsonException(throwMessage);
            }
        }

        public static string Serialize<T>(in T? source) {
            return JsonSerializer.Serialize(source, DefaultOptions);
        }

        #region Converters
        private sealed class InstallSourceJsonConverter : JsonConverter<InstallSource> {
            public override InstallSource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                return new(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, InstallSource value, JsonSerializerOptions options) {
                writer.WriteStringValue(value.ToString());
            }
        }

        private class UnibuildTypeJsonConverter : JsonConverter<UnibuildType> {
            public override UnibuildType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                return EnumHelper.ToUnibuild(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, UnibuildType value, JsonSerializerOptions options) {
                writer.WriteStringValue(EnumHelper.ToString(value));
            }
        }

        private class RegionsJsonConverter : JsonConverter<Regions> {
            public override Regions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                return EnumHelper.ToRegions(JsonSerializer.Deserialize<string[]>(ref reader, options));
            }

            public override void Write(Utf8JsonWriter writer, Regions value, JsonSerializerOptions options) {
                JsonSerializer.Serialize(writer, EnumHelper.ToList(value), options);
            }
        }

        private class BuildTaskTypeConverter : JsonConverter<BuildTaskType> {
            public override BuildTaskType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                return EnumHelper.ToBuildTaskType(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, BuildTaskType value, JsonSerializerOptions options) {
                writer.WriteStringValue(value.ToString());
            }
        }

        private class BuildTaskListConverter : JsonConverter<List<BuildTask>> {
            public override List<BuildTask> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                var arr = JsonSerializer.Deserialize<JsonArray>(ref reader, options);
                if (arr is null) {
                    return [];
                }

                var list = new List<BuildTask>(arr.Count);

                foreach (var node in arr) {
                    if (node is null)
                        continue;

                    var task = BuildTask.Deserialize(node, options);

                    if (task is not null)
                        list.Add(task);
                }

                return list;
            }

            public override void Write(Utf8JsonWriter writer, List<BuildTask> value, JsonSerializerOptions options) {
                var arr = new JsonArray();

                foreach (var task in value) {
                    arr.Add(JsonSerializer.SerializeToNode(task, task.GetType(), options));
                }

                JsonSerializer.Serialize(writer, arr, options);
            }
        }

        #endregion
    }
}
