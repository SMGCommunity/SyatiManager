using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;
using SyatiManager.Source.Common;
using SyatiManager.Source.Common.Helpers;
using SyatiManager.Source.Solutions;

namespace SyatiManager.UI.Windows {
    public partial class EditIgnoredWindow : AppWindow {
        public static Solution? Solution {
            get => SyatiCore.Instance.Solution;
        }

        public EditIgnoredWindow() {
            InitializeComponent();
            AvaloniaHelper.ConfigureWindow(this, AcrylicBorder, TitleBarPanel);
        }

        protected override void OnClosing(WindowClosingEventArgs e) {
            IgnoreEntriesControl.ItemsSource = null;
        }

        public void AddEntry() {
            if (Solution is null || InputIgnoredEntry.Text is null)
                return;

            Solution.AddIgnoreEntry(InputIgnoredEntry.Text);
            InputIgnoredEntry.Text = string.Empty;
        }
    }
}