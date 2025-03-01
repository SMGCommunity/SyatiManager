using Avalonia.Collections;
using Avalonia.Input;
using SyatiManager.Source.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyatiManager.Source.Libraries {
    public class ModuleLibrary : LibraryBase<ModuleLibrary, LibraryModule> {
        public List<string> CategoryList { get; private set; }

        public ModuleLibrary(string path) : base(path) {
            mSelectionHolder.SelectionChanged += SelectionHolderChanged;
        }

        public LibraryModule? this[string id] {
            get => mItems?.FirstOrDefault(m => m.FolderName.Equals(id));
        }

        public override void Load() {
            JsonHelper.Deserialize(out LibraryModuleData[] dataCollection, File.ReadAllText(mPath), "The parsed Module Library JSON is null.");

            mItems = new(dataCollection.Length);

            foreach (var data in dataCollection) {
                Add(new(data));
            }

            CategoryList = mItems.SelectMany(m => m.Categories).Distinct().ToList();
            CategoryList.Insert(0, "All");
        }

        public override async Task Update() {
            try {
                using var client = new HttpClient();

                using var fs = File.OpenWrite(mPath);
                using var ms = await client.GetStreamAsync("https://raw.githubusercontent.com/SMGCommunity/SyatiManager/refs/heads/main/SyatiManager/Components/Modules.json");
                
                await ms.CopyToAsync(fs);
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while updating the preset library", ex);            
            }
        }

        private void Add(LibraryModule module) {
            module.PointerPressed += ModuleClicked;
            mItems.Add(module);
        }

        private void ModuleClicked(object? sender, PointerPressedEventArgs e) {
            if (sender is LibraryModule module) {
                mSelectionHolder.Select(module);

                RaisePropertyChanged(SelectedItemProperty, null, module);
            }
        }

        private void SelectionHolderChanged(LibraryModule module, bool value) {
            module.SetSelect(value);
        }
    }

    /*
    public class ModuleLibrary : AvaloniaObject {
        private readonly string mPath;
        private readonly SelectionHolder<LibraryModule> mSelectionHolder;

        public static readonly StyledProperty<LibraryModules> ModulesProperty =
            AvaloniaProperty.Register<ModuleLibrary, LibraryModules>(nameof(Modules));

        public static readonly DirectProperty<ModuleLibrary, LibraryModule?> SelectedModuleProperty =
            AvaloniaProperty.RegisterDirect<ModuleLibrary, LibraryModule?>(nameof(SelectedModule), o => o.SelectedModule);

        public LibraryModules Modules {
            get => GetValue(ModulesProperty);
            private set => SetValue(ModulesProperty, value);
        }

        public LibraryModule? SelectedModule {
            get => mSelectionHolder.SelectedItem;
        }

        public ModuleLibrary(string path) {
            mSelectionHolder = new();
            mSelectionHolder.SelectionChanged += SelectionHolderChanged;

            mPath = path;
        }

        public void Load() {
            JsonHelper.Deserialize(out LibraryModuleData[] dataCollection, File.ReadAllText(mPath), "The parsed Module Library JSON is null.");

            Modules = new(dataCollection.Length);

            foreach (var data in dataCollection) {
                AddModule(new(data));
            }
        }

        public async Task Update() {
            Console.WriteLine("ModuleLibrary.Update() does not have proper implementation. Modules.json.legacy will be created instead.");
            // await Task.Delay(1000);

            using var client = new HttpClient();
            File.WriteAllText(mPath + ".legacy", await client.GetStringAsync("https://raw.githubusercontent.com/SMGCommunity/SyatiManager/refs/heads/main/installable_modules.json"));
            return;
        }

        private void AddModule(LibraryModule module) {
            module.PointerPressed += ModuleClicked;
            Modules.Add(module);
        }

        public LibraryModule? this[string id] {
            get => Modules?.FirstOrDefault(m => m.FolderName.Equals(id));
        }

        private void ModuleClicked(object? sender, PointerPressedEventArgs e) {
            if (sender is LibraryModule module) {
                mSelectionHolder.Select(module);
                RaisePropertyChanged(SelectedModuleProperty, null, module);
            }
        }

        private void SelectionHolderChanged(LibraryModule module, bool value) {
            module.SetSelect(value);
        }
    }
    */
}
