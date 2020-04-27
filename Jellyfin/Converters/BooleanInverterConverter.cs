using System;
using Windows.UI.Xaml.Data;

namespace Jellyfin.Converters
{
    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is bool))
            {
                return null;
            }

            return !(bool)value;

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
