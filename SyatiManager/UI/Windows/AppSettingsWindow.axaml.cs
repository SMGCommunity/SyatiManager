using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;
using SyatiManager.Source.Common;
using SyatiManager.Source.Common.Helpers;
using System;
using System.Threading.Tasks;

namespace SyatiManager.UI.Windows {
    public partial class AppSettingsWindow : AppWindow {
        public static SyatiCore Core {
            get => SyatiCore.Instance;
        }

        public AppSettingsWindow() {
            InitializeComponent();
            AvaloniaHelper.ConfigureWindow(this, AcrylicBorder, TitleBarPanel);
        }

        public async Task SelectSyatiPath() {
            var folders = await AvaloniaHelper.OpenFolderPicker(this, AvaloniaHelper.CommonFolderPickerOptions);

            if (folders.Count > 0) {
                Core.SyatiPath = folders[0].Path.AbsolutePath;
                Console.WriteLine($"Set Syati path to {folders[0].Path.AbsolutePath}");
            }
        }

        public async Task SelectBuildToolFolder() {
            var folders = await AvaloniaHelper.OpenFolderPicker(this, AvaloniaHelper.CommonFolderPickerOptions);

            if (folders.Count > 0) {
                Core.BuildToolFolder = folders[0].Path.AbsolutePath;
                Console.WriteLine($"Set Build Tool path to {folders[0].Path.AbsolutePath}");
            }
        }

        protected override void OnClosing(WindowClosingEventArgs e) {
            Core.SaveSettings();
            base.OnClosing(e);
        }
    }
}