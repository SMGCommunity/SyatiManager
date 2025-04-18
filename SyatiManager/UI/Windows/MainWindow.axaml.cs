using Avalonia.Controls;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Windowing;
using SyatiManager.Source.Common;
using SyatiManager.Source.Common.Helpers;
using SyatiManager.Source.Solutions;
using System;
using System.Threading.Tasks;

namespace SyatiManager.UI.Windows {
    public partial class MainWindow : AppWindow {
        public static SyatiCore Core {
            get => SyatiCore.Instance;
        }

        public static Solution? Solution {
            get => SyatiCore.Instance.Solution;
        }

        public MainWindow() {
            InitializeComponent();

            AvaloniaHelper.ConfigureWindow(this, AcrylicBorder, TitleBarPanel);
            USACheckbox.IsCheckedChanged += (_, _) => Solution?.SetRegion(Regions.USA, USACheckbox.IsChecked ?? false);
            PALCheckbox.IsCheckedChanged += (_, _) => Solution?.SetRegion(Regions.PAL, PALCheckbox.IsChecked ?? false);
            JPNCheckbox.IsCheckedChanged += (_, _) => Solution?.SetRegion(Regions.JPN, JPNCheckbox.IsChecked ?? false);
            KORCheckbox.IsCheckedChanged += (_, _) => Solution?.SetRegion(Regions.KOR, KORCheckbox.IsChecked ?? false);
            TWNCheckbox.IsCheckedChanged += (_, _) => Solution?.SetRegion(Regions.TWN, TWNCheckbox.IsChecked ?? false);

            Loaded += WindowLoaded;
            Closing += WindowClosing;

            //LoadSolution("C:\\CustomProjects\\SMG\\SyatiManager\\SyatiManager\\bin\\Debug\\net8.0\\Test\\Test.syt");
        }

        public async Task NewSolution() {
            var path = await AvaloniaHelper.SaveFilePicker(this, Solution.NewOptions);

            if (path is not null) {
                Core.LoadSolution(path.Path.LocalPath);
            }
        }

        public async Task OpenSolution() {
            var files = await AvaloniaHelper.OpenFilePicker(this, Solution.OpenOptions);

            if (files is not null && files.Count > 0) {
                Core.LoadSolution(files[0].Path.LocalPath);
                UpdateRegionCheckboxes();
            }
        }

        public async Task SelectModulesPath() {
            if (Solution is null)
                return;

            var folders = await AvaloniaHelper.OpenFolderPicker(this, AvaloniaHelper.CommonFolderPickerOptions);

            if (folders.Count > 0) {
                Solution.ModulesPath = folders[0].Path.LocalPath;
                Console.WriteLine($"Set modules path to {Solution.ModulesPath} [{Solution.Modules.Count} modules]");

                Solution.Save();
            }
        }

        public async Task SelectOutputPath() {
            if (Solution is null)
                return;

            var folders = await AvaloniaHelper.OpenFolderPicker(this, AvaloniaHelper.CommonFolderPickerOptions);

            if (folders.Count > 0) {
                Solution.OutputPath = folders[0].Path.LocalPath;
                Console.WriteLine($"Set output path to {Solution.OutputPath}");

                Solution.Save();
            }
        }

        public void OpenEditIgnoredWindow() {
            if (Solution is null)
                return;

            new EditIgnoredWindow().Show(this);
        }

        public void OpenAppSettingsWindow() {
            new AppSettingsWindow().Show(this);
        }

        public void OpenModuleLibWindow() {
            if (Solution is null)
                return;

            new ModuleLibWindow().Show(this);
        }

        public void OpenPresetLibWindow() {
            if (Solution is null)
                return;

            new PresetLibWindow().Show(this);
        }

        private async void WindowLoaded(object? sender, RoutedEventArgs e) {
            double SplashIncAmount = 100 / (Core.AutoUpdateSyati ? 6 : 5);

            Splash.Message = "Updating Module Library...";
            await SyatiCore.ModuleLibrary.Update();
            Splash.Value += SplashIncAmount;

            Splash.Message = "Loading Module Library...";
            SyatiCore.ModuleLibrary.Load();
            Splash.Value += SplashIncAmount;

            Splash.Message = "Updating Preset Library...";
            await SyatiCore.PresetLibrary.Update();
            Splash.Value += SplashIncAmount;

            Splash.Message = "Loading Preset Library...";
            SyatiCore.PresetLibrary.Load();
            Splash.Value += SplashIncAmount;

            if (Core.AutoUpdateSyati) {
                Splash.Message = "Updating Syati...";
                await Core.UpdateSyati();
                Splash.Value += SplashIncAmount;
            }

            Splash.Message = "Processing Arguments...";
            await App.ProcessArgs();
            UpdateRegionCheckboxes();
            Splash.Value = 100;

            Console.WriteLine("Initialized SyatiManager.");            
        }

        private void WindowClosing(object? sender, WindowClosingEventArgs e) {
            Solution?.Save();
        }

        private void UpdateRegionCheckboxes() {
            if (Solution is null)
                return;

            USACheckbox.IsChecked = Solution.GetRegion(Regions.USA);
            PALCheckbox.IsChecked = Solution.GetRegion(Regions.PAL);
            JPNCheckbox.IsChecked = Solution.GetRegion(Regions.JPN);
            KORCheckbox.IsChecked = Solution.GetRegion(Regions.KOR);
            TWNCheckbox.IsChecked = Solution.GetRegion(Regions.TWN);
        }
    }
}