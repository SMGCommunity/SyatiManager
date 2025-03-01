using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using FluentAvalonia.UI.Windowing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyatiManager.Source.Common.Helpers {
    public static class AvaloniaHelper {
        public static readonly FolderPickerOpenOptions CommonFolderPickerOptions = new() {
            AllowMultiple = false,
            Title = "Select a folder"
        };

        public static readonly Bitmap ApplicationIcon = new(AssetLoader.Open(new("avares://SyatiManager/Assets/Icon.png")));

        public static void ConfigureWindow(AppWindow win, ExperimentalAcrylicBorder acrylicBorder, Panel? titleBarPanel = null) {
            if (OperatingSystem.IsLinux()) {
                if (win.Content is Grid grid)
                    grid.RowDefinitions[0].Height = new(0);

                if (titleBarPanel is not null)
                    titleBarPanel.IsVisible = false;
            }
            else if (titleBarPanel is not null) {
                ((Image)titleBarPanel.Children[0]).Source = ApplicationIcon;
            }

            acrylicBorder.IsVisible = win.ActualTransparencyLevel == WindowTransparencyLevel.AcrylicBlur;

            win.TitleBar.Height = 35;
            win.TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
            win.TitleBar.ExtendsContentIntoTitleBar = true;

            win.TitleBar.ButtonHoverBackgroundColor = GetResource<Color>("HighlightHoverColor");
            win.TitleBar.ButtonPressedBackgroundColor = GetResource<Color>("AccentColor");
            win.TitleBar.ButtonInactiveForegroundColor = Colors.White;
        }

        public static T? GetResource<T>(object key) {
            if (Application.Current!.TryGetResource(key, ThemeVariant.Dark, out var output)) {
                if (output is T res) {
                    return res;
                }
            }

            return default;
        }

        public static async Task<IReadOnlyList<IStorageFile>> OpenFilePicker(TopLevel topLevel, FilePickerOpenOptions options) {
            return await topLevel.StorageProvider.OpenFilePickerAsync(options);
        }

        public static async Task<IStorageFile?> SaveFilePicker(TopLevel topLevel, FilePickerSaveOptions options) {
            return await topLevel.StorageProvider.SaveFilePickerAsync(options);
        }

        public static async Task<IReadOnlyList<IStorageFolder>> OpenFolderPicker(TopLevel topLevel, FolderPickerOpenOptions options) {
            return await topLevel.StorageProvider.OpenFolderPickerAsync(options);
        }
    }
}
