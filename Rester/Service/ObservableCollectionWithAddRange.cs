using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Rester.Service
{
    internal class ObservableCollectionWithAddRange<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ClearAndAddRange(IEnumerable<T> items)
        {
            Items.Clear();
            AddRange(items);
        }
    }
}