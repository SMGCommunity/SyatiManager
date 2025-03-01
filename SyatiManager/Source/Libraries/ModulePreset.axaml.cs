using Avalonia;
using Avalonia.Controls;
using SyatiManager.Source.Common;
using SyatiManager.Source.Solutions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SyatiManager.Source.Libraries {
    public partial class ModulePreset : UserControl {
        public static readonly StyledProperty<string> PresetNameProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(PresetName));

        public static readonly StyledProperty<string> DescriptionProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(Description));

        public static readonly StyledProperty<string> AuthorProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(Author));

        public static readonly StyledProperty<InstallSource> InstallProperty =
            AvaloniaProperty.Register<ModuleInfo, InstallSource>(nameof(Install));

        public string PresetName {
            get => GetValue(PresetNameProperty);
            set => SetValue(PresetNameProperty, value);
        }

        public string Description {
            get => GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public string Author {
            get => GetValue(AuthorProperty);
            set => SetValue(AuthorProperty, value);
        }

        public InstallSource Install {
            get => GetValue(InstallProperty);
            set => SetValue(InstallProperty, value);
        }

        public ModulePreset() {
            InitializeComponent();
        }

        public ModulePreset(ModulePresetData data) : this() {
            PresetName = data.Name;
            Description = data.Description;
            Author = data.Author;
            Install = data.Install;
        }

        public void SetSelect(bool value) {
            PseudoClasses.Set(":selected", value);
        }

        public void OpenLink() {
            Process.Start(new ProcessStartInfo() {
                FileName = Install.Url,
                UseShellExecute = true,
            });
        }
    }

    public class ModulePresetData {
        public string Name
            { get; set; } = string.Empty;

        public string Description
            { get; set; } = string.Empty;

        public string Author
            { get; set; } = string.Empty;

        [AllowNull]
        public InstallSource Install
            { get; set; }
    }
}