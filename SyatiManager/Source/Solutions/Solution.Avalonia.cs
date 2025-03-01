using Avalonia;
using Avalonia.Collections;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using System.Collections.Generic;

namespace SyatiManager.Source.Solutions {
    public partial class Solution {
        private static readonly IReadOnlyList<FilePickerFileType> PickerExtension =
            [new("Solution Files") { Patterns = ["*.syt"] }];

        public static readonly FilePickerOpenOptions OpenOptions = new() {
            AllowMultiple = false,
            FileTypeFilter = PickerExtension,
            Title = "Open Solution"
        };

        public static readonly FilePickerSaveOptions NewOptions = new() {
            FileTypeChoices = PickerExtension,
            ShowOverwritePrompt = true,
            Title = "New Solution"
        };

        public static readonly DirectProperty<Solution, AvaloniaList<ModuleInfo>> ModulesProperty =
            AvaloniaProperty.RegisterDirect<Solution, AvaloniaList<ModuleInfo>>(nameof(Modules), o => o.Modules);

        public static readonly DirectProperty<Solution, AvaloniaList<IgnoreEntry>> IgnoreEntriesProperty =
            AvaloniaProperty.RegisterDirect<Solution, AvaloniaList<IgnoreEntry>>(nameof(IgnoreEntries), o => o.IgnoreEntries);

        public static readonly DirectProperty<Solution, ModuleInfo?> SelectedModuleProperty =
            AvaloniaProperty.RegisterDirect<Solution, ModuleInfo?>(nameof(SelectedModule), o => o.SelectedModule);

        public static readonly DirectProperty<Solution, string> ModulesPathProperty =
            AvaloniaProperty.RegisterDirect<Solution, string>(nameof(ModulesPath), o => o.RelativeModulesPath);

        public static readonly DirectProperty<Solution, string> OutputPathProperty =
            AvaloniaProperty.RegisterDirect<Solution, string>(nameof(OutputPath), o => o.RelativeOutputPath);

        private void ModuleClicked(object? sender, PointerPressedEventArgs e) {
            if (sender is ModuleInfo module)
                SelectModule(module);
        }
    }
}
