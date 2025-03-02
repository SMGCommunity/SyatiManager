using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;
using SyatiManager.Source.Common;
using SyatiManager.Source.Common.Helpers;
using SyatiManager.Source.Libraries;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyatiManager.UI.Windows {
    public partial class PresetLibWindow : AppWindow {
        public static PresetLibrary PresetLibrary {
            get => SyatiCore.PresetLibrary;
        }

        public PresetLibWindow() {
            InitializeComponent();
            AvaloniaHelper.ConfigureWindow(this, AcrylicBorder, TitleBarPanel);

            NameSearchBox.TextChanged += (_, _) => Search(NameSearchBox.Text);
            FolderNameInput.TextChanged += (_, _) => {
                if (FolderNameInput.Text is not null)
                    FolderNameInput.Text = FolderNameRegex().Replace(FolderNameInput.Text, "");
            };
        }

        protected override void OnClosing(WindowClosingEventArgs e) {
            ResetPresetVisibility();
            PresetsControl.ItemsSource = null;
        }

        private static void Search(string? text) {
            if (string.IsNullOrEmpty(text)) {
                ResetPresetVisibility();
                return;
            }

            foreach (var module in PresetLibrary.Items) {
                module.IsVisible = module.PresetName.Contains(text, StringComparison.OrdinalIgnoreCase);
            }
        }

        private static void ResetPresetVisibility() {
            foreach (var module in PresetLibrary.Items) {
                module.IsVisible = true;
            }
        }

        public void ShowInputWindow() {
            if (PresetLibrary.SelectedItem is null)
                return;

            InputWindow.IsVisible = true;
        }

        public void HideInputWindow() {
            InputWindow.IsVisible = false;
        }

        public async Task Create() {
            if (PresetLibrary.SelectedItem is null)
                return;

            if (string.IsNullOrEmpty(FolderNameInput.Text)) {
                Console.WriteLine("Folder name is empty.");
                return;
            }

            await SyatiCore.Instance.InstallPreset(PresetLibrary.SelectedItem, new() {
                FolderName = FolderNameInput.Text,
                Name = NameInput.Text ?? string.Empty,
                Author = AuthorInput.Text ?? string.Empty,
                Description = DescriptionInput.Text ?? string.Empty,
            });
        }

        [GeneratedRegex("[^a-zA-Z0-9.\\-_]")]
        private static partial Regex FolderNameRegex();
    }
}