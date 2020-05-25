using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Jellyfin.Converters
{
    public sealed class CollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IEnumerable<object> enumerable;

            if (value is IEnumerable<object> l)
            {
                enumerable = l;
            }
            else
            {
                return Visibility.Collapsed;
            }

            return !enumerable.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility?) value == Visibility.Visible;
        }
    }
}