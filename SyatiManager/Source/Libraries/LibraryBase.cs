using Avalonia;
using Avalonia.Collections;
using SyatiManager.Source.Common;
using System.Threading.Tasks;

namespace SyatiManager.Source.Libraries {
    public abstract class LibraryBase<TOwner, TItem> : AvaloniaObject
        where TOwner : LibraryBase<TOwner, TItem>
        where TItem : class
    {
        protected readonly string mPath;
        protected AvaloniaList<TItem> mItems;
        protected readonly SelectionHolder<TItem> mSelectionHolder;

        public static readonly DirectProperty<TOwner, AvaloniaList<TItem>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<TOwner, AvaloniaList<TItem>>(nameof(Items), o => o.Items);

        public static readonly DirectProperty<TOwner, TItem?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<TOwner, TItem?>(nameof(SelectedItem), o => o.SelectedItem);

        public AvaloniaList<TItem> Items {
            get => mItems;
        }

        public TItem? SelectedItem {
            get => mSelectionHolder.SelectedItem;
        }

        public LibraryBase(string path) {
            mSelectionHolder = new();
            mPath = path;
        }

        public abstract void Load();

        public virtual async Task Update() { }
    }
}
