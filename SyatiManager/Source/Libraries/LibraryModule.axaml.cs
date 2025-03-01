using Avalonia;
using Avalonia.Controls;
using SyatiManager.Source.Common;
using SyatiManager.Source.Solutions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SyatiManager.Source.Libraries {
    public partial class LibraryModule : UserControl {
        public static readonly StyledProperty<string> ModuleNameProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(ModuleName));

        public static readonly StyledProperty<string> FolderNameProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(FolderName));

        public static readonly StyledProperty<string> DescriptionProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(Description));

        public static readonly StyledProperty<string> AuthorProperty =
            AvaloniaProperty.Register<ModuleInfo, string>(nameof(Author));

        public static readonly StyledProperty<string[]> CategoriesProperty =
            AvaloniaProperty.Register<ModuleInfo, string[]>(nameof(Categories));

        public static readonly StyledProperty<InstallSource> InstallProperty =
            AvaloniaProperty.Register<ModuleInfo, InstallSource>(nameof(Install));

        public string ModuleName {
            get => GetValue(ModuleNameProperty);
            set => SetValue(ModuleNameProperty, value);
        }

        public string FolderName {
            get => GetValue(FolderNameProperty);
            set => SetValue(FolderNameProperty, value);
        }

        public string Description {
            get => GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public string Author {
            get => GetValue(AuthorProperty);
            set => SetValue(AuthorProperty, value);
        }

        public string[] Categories {
            get => GetValue(CategoriesProperty);
            set => SetValue(CategoriesProperty, value);
        }

        public InstallSource Install {
            get => GetValue(InstallProperty);
            set => SetValue(InstallProperty, value);
        }

        public LibraryModule() {
            InitializeComponent();
        }

        public LibraryModule(LibraryModuleData data) : this() {
            ModuleName = data.Name;
            FolderName = data.FolderName;
            Description = data.Description;
            Author = data.Author;
            Categories = data.Categories;
            Install = data.Install;
        }

        public void SetSelect(bool value) {
            PseudoClasses.Set(":selected", value);
        }

        public async void InstallModule() {
            await SyatiCore.Instance.InstallModule(this);
        }

        public void OpenLink() {
            Process.Start(new ProcessStartInfo() {
                FileName = Install.Url,
                UseShellExecute = true,
            });
        }
    }

    public class LibraryModuleData {
        public string Name
            { get; set; } = string.Empty;

        public string FolderName
            { get; set; } = string.Empty;

        public string Description
            { get; set; } = string.Empty;

        public string Author
            { get; set; } = string.Empty;

        [AllowNull]
        public string[] Categories
            { get; set; }

        [AllowNull]
        public InstallSource Install
            { get; set; }
    }
}