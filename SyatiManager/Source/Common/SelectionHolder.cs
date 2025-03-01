using System;

namespace SyatiManager.Source.Common {
    public class SelectionHolder<T> where T : class {
        private T? _selectedItem;

        /// <summary>
        /// Event that gets called when the <see cref="Select(T?)"/> method is used.
        /// </summary>
        public event Action<T, bool>? SelectionChanged;

        /// <summary>
        /// Gets the current selected item.
        /// </summary>
        public T? SelectedItem => _selectedItem;

        /// <summary>
        /// Creates a new <see cref="SelectionHolder{T}"/>.
        /// </summary>
        public SelectionHolder() { }

        /// <summary>
        /// Creates a new <see cref="SelectionHolder{T}"/> with an initial selected item.
        /// </summary>
        /// <param name="item">The item to select. If set to null, the selection will be cleared.</param>
        public SelectionHolder(T? item) {
            Select(item);
        }

        /// <summary>
        /// Selects an item and calls <see cref="SelectionChanged"/>.
        /// </summary>
        /// <param name="item">The item to select. If set to null, the selection will be cleared.</param>
        public void Select(T? item) {
            if (_selectedItem is not null) {
                SelectionChanged?.Invoke(_selectedItem, false);
                _selectedItem = null;
            }

            if (item is not null) {
                SelectionChanged?.Invoke(item, true);
                _selectedItem = item;
            }
        }
    }
}
