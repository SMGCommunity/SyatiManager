using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;
using SyatiManager.Source.Common;
using SyatiManager.Source.Common.Helpers;
using SyatiManager.Source.Libraries;
using System;
using System.Linq;

namespace SyatiManager.UI.Windows {
    public partial class ModuleLibWindow : AppWindow {
        public static ModuleLibrary ModuleLibrary {
            get => SyatiCore.ModuleLibrary;
        }

        public ModuleLibWindow() {
            InitializeComponent();
            AvaloniaHelper.ConfigureWindow(this, AcrylicBorder, TitleBarPanel);

            NameSearchBox.TextChanged += (_, _) => Search(NameSearchBox.Text, CategorySearch.SelectedIndex);
            CategorySearch.SelectionChanged += (_, _) => Search(NameSearchBox.Text, CategorySearch.SelectedIndex);
        }

        protected override void OnClosing(WindowClosingEventArgs e) {
            ResetModuleVisibility();
            ModulesControl.ItemsSource = null;
        }

        private static void Search(string? text, int categoryIndex) {
            if (string.IsNullOrEmpty(text) && categoryIndex == 0) {
                ResetModuleVisibility();
                return;
            }

            foreach (var module in ModuleLibrary.Items) {
                if (text is not null) {
                    module.IsVisible = module.ModuleName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                                       module.FolderName.Contains(text, StringComparison.OrdinalIgnoreCase);

                    if (!module.IsVisible)
                        continue;
                }

                module.IsVisible = categoryIndex == 0 || module.Categories.Contains(ModuleLibrary.CategoryList[categoryIndex]);
            }
        }

        private static void ResetModuleVisibility() {
            foreach (var module in ModuleLibrary.Items) {
                module.IsVisible = true;
            }
        }
    }
}