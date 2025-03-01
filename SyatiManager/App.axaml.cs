using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluentAvalonia.Core;
using SyatiManager.Source.Common;
using SyatiManager.UI.Windows;
using System;
using System.Threading.Tasks;

namespace SyatiManager {
    public partial class App : Application {
        private static string[]? _args;

        public static SyatiCore Core {
            get => SyatiCore.Instance;
        }

        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                Core.LoadSettings();

                if (desktop.Args is not null) {
                    if (desktop.Args.Contains("-h") ||
                        desktop.Args.Contains("--help")) {
                        ShowHelp();
                        Environment.Exit(0);
                    }

                    _args = desktop.Args;

                    if (desktop.Args.Contains("--no-gui")) {
                        await ProcessArgs();
                        Environment.Exit(0);
                    }
                }

                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        public static async Task ProcessArgs() {
            if (_args is null || _args.Length == 0)
                return;

            Core.LoadSolution(_args[0]);

            if (!Core.IsSolutionOpen || _args.Length == 1)
                return;

            if (_args.Contains("-b")  ||
                _args.Contains("--build")) {
                await Core.BuildCode();
            }

            if (_args.Contains("-l") ||
                _args.Contains("--loader")) {
                await Core.BuildLoader();
            }

            _args = null;
        }

        private static void ShowHelp() {
            Console.WriteLine("""
                Usage: SyatiManager.exe <Input> [Options]

                Arguments:
                  <Input>       The path of a Syati Solution (.syt) file

                Options:
                  -h, --help    Displays this message
                  -b, --build   Build solution code
                  -l, --loader  Build loader
                  --no-gui      Do not show the GUI
                """);
        }
    }
}