using Avalonia;
using Avalonia.Controls;
using SyatiManager.Source.Common;

namespace SyatiManager.Source.Solutions {
    public partial class IgnoreEntry : UserControl {
        public static readonly DirectProperty<IgnoreEntry, string> FolderNameProperty =
            AvaloniaProperty.RegisterDirect<IgnoreEntry, string>(nameof(FolderName), o => o.FolderName, (o, v) => o.FolderName = v);

        private string _folderName;

        public string FolderName {
            get => _folderName;
            set => SetAndRaise(FolderNameProperty, ref _folderName, value);
        }

        public IgnoreEntry() {
            InitializeComponent();
        }

        public IgnoreEntry(string folderName) : this() {
            FolderName = folderName;
        }

        public void Remove() {
            SyatiCore.Instance.Solution?.RemoveIgnoreEntry(this);
        }
    }
}