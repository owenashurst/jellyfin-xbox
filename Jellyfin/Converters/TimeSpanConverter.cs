using System;
using Windows.UI.Xaml.Data;

namespace Jellyfin.Converters
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is TimeSpan))
            {
                return null;
            }

            TimeSpan ts = (TimeSpan)value;
            return $"{ts.Hours}:{ts.Minutes.ToString().PadLeft(2, '0')}:{ts.Seconds.ToString().PadLeft(2, '0')}";

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
