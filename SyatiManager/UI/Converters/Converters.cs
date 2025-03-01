using Avalonia.Data.Converters;
using System.Collections.Generic;

namespace SyatiManager.UI.Converters {
    public static class LocalConverters {
        public static FuncValueConverter<IEnumerable<string?>, string> JoinEnumerable { get; } =
             new FuncValueConverter<IEnumerable<string?>, string>(a => a is null ? string.Empty : string.Join(", ", a));

        public static FuncValueConverter<bool?, bool> NullBool { get; } =
             new FuncValueConverter<bool?, bool>(a => a ?? false);
    }
}
