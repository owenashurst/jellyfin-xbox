using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Extensions
{
    public class ObservableCollectionEx<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Unsubscribe(e.OldItems);
            Subscribe(e.NewItems);
            base.OnCollectionChanged(e);
        }

        private void Subscribe(IList newItems)
        {
            if (newItems != null)
            {
                foreach (T item in newItems)
                {
                    item.PropertyChanged += ContainedElementChanged;
                }
            }
        }

        private void Unsubscribe(IList removedItems)
        {
            if (removedItems != null)
            {
                foreach (T element in removedItems)
                {
                    element.PropertyChanged -= ContainedElementChanged;
                }
            }
        }

        public ObservableCollectionEx()
        {
            
        }

        public ObservableCollectionEx(IEnumerable<T> collection) : base(collection)
        {
            
        }

        public ObservableCollectionEx(IList<T> collection) : base(collection)
        {

        }

        private void ContainedElementChanged(object x, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
    }
}
