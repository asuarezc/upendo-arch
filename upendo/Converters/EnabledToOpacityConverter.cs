using System;
using System.Globalization;
using Xamarin.Forms;

namespace upendo.Converters
{
    public class EnabledToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double opacityWhenDisabled = parameter != null && parameter is double doubleParameter
                ? doubleParameter
                : 0.5d;

            if (value == null)
                return opacityWhenDisabled;

            if (!bool.TryParse(value.ToString(), out bool enabled))
                return 1d;

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
