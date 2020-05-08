using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Jellyfin.Converters
{
    public sealed class GreaterThanZeroToVisibilityConverter : IValueConverter
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

            if (string.IsNullOrEmpty(text))
            {
                return Visibility.Collapsed;
            }

            if (!int.TryParse(text, out int result))
            {
                return Visibility.Collapsed;
            }

            return result <= 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility?) value == Visibility.Visible;
        }
    }
}