using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SyatiManager.Source.Common.Helpers {
    public static class EnumHelper {
        public static Regions ToRegions(IEnumerable<string>? names) {
            var regions = Regions.None;

            if (names is not null) {
                foreach (var region in names) {
                    regions |= ToRegion(region);
                }
            }

            return regions;
        }

        public static Regions ToRegion(string str) {
            return str switch {
                "USA" => Regions.USA,
                "PAL" => Regions.PAL,
                "JPN" => Regions.JPN,
                "KOR" => Regions.KOR,
                "TWN" => Regions.TWN,
                _ => Regions.None,
            }; ;
        }

        public static char GetRegionLetter(string region) {
            return region switch {
                "USA" => 'E',
                "PAL" => 'P',
                "JPN" => 'J',
                "KOR" => 'K',
                "TWN" => 'W',
                _ => throw new FormatException("Invalid Region.")
            };
        }

        public static List<string> ToList(this Regions regions) {
            if (regions == Regions.None)
                return [];

            var names = new List<string>(5);

            if ((regions & Regions.USA) != 0) names.Add("USA");
            if ((regions & Regions.PAL) != 0) names.Add("PAL");
            if ((regions & Regions.JPN) != 0) names.Add("JPN");
            if ((regions & Regions.KOR) != 0) names.Add("KOR");
            if ((regions & Regions.TWN) != 0) names.Add("TWN");

            return names;
        }

        public static UnibuildType ToUnibuild(string str) {
            return str switch {
                "Never" => UnibuildType.Never,
                "Always" => UnibuildType.Always,
                "Auto" => UnibuildType.Auto,
                _ => throw new FormatException($"Unknown UnibuildType \"{str}\".")
            };
        }

        public static string ToString(this UnibuildType type) {
            return type.ToString();
        }

        public static BuildTaskType ToBuildTaskType(string str) {
            return str switch {
                "Copy"        => BuildTaskType.Copy,
                "Command"     => BuildTaskType.Command,
                "Arc_Append"  => BuildTaskType.Arc_Append,
                "Bcsv_Append" => BuildTaskType.Bcsv_Append,
                "Bcsv_Write"  => BuildTaskType.Bcsv_Write,
                _ => throw new JsonException($"Unknown BuildTask: {str}"),
            }; ;
        }
    }
}
