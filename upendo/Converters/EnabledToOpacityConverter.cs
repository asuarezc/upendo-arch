using System;
using System.Globalization;
using Xamarin.Forms;

namespace upendo.Converters
{
    public class EnabledToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new InvalidOperationException("value must be bool");

            if (!bool.TryParse(value.ToString(), out bool enabled))
                return 1d;

            double opacityWhenDisabled = parameter != null && parameter is double doubleParameter
                ? doubleParameter
                : 0.5d;

            return enabled
                ? 1d
                : opacityWhenDisabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException($"{nameof(EnabledToOpacityConverter)} cannot convert back. Is OneWay.");
        }
    }
}
