using System;

namespace SyatiManager.Source.Common {
    public enum UnibuildType {
        Never,
        Auto,
        Always
    }

    [Flags]
    public enum Regions {
        None = 0,
        All = USA | PAL | JPN | KOR | TWN,

        USA = 1,
        PAL = 2,
        JPN = 4,
        KOR = 8,
        TWN = 16,
    }
}
