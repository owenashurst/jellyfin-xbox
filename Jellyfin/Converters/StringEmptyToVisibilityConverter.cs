using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Jellyfin.Converters
{
    public sealed class StringEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string text;

            if (value is string s)
            {
                text = s;
            }
            else
            {
                return Visibility.Collapsed;
            }

            return string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility?) value == Visibility.Visible;
        }
    }
}