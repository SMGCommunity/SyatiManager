using Avalonia.Input;
using SyatiManager.Source.Common.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyatiManager.Source.Libraries {
    public class PresetLibrary : LibraryBase<PresetLibrary, ModulePreset> {
        public PresetLibrary(string path) : base(path) {
            mSelectionHolder.SelectionChanged += SelectionHolderChanged;
        }

        public override void Load() {
            JsonHelper.Deserialize(out ModulePresetData[] dataCollection, File.ReadAllText(mPath), "The parsed Module Library JSON is null.");

            mItems = new(dataCollection.Length);

            foreach (var data in dataCollection) {
                Add(new(data));
            }
        }

        public override async Task Update() {
            try {
                using var client = new HttpClient();

                using var fs = File.OpenWrite(mPath);
                using var ms = await client.GetStreamAsync("https://raw.githubusercontent.com/SMGCommunity/SyatiManager/refs/heads/main/SyatiManager/Components/Presets.json");

                await ms.CopyToAsync(fs);
            }
            catch (Exception ex) {
                IOHelper.WriteError("Error while updating the preset library", ex);            
            }
        }

        private void Add(ModulePreset module) {
            module.PointerPressed += ModuleClicked;
            mItems.Add(module);
        }

        private void ModuleClicked(object? sender, PointerPressedEventArgs e) {
            if (sender is ModulePreset module) {
                mSelectionHolder.Select(module);

                RaisePropertyChanged(SelectedItemProperty, null, module);
            }
        }

        private void SelectionHolderChanged(ModulePreset module, bool value) {
            module.SetSelect(value);
        }
    }
}
