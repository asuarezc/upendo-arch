using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace upendo.Converters
{
    public class ObjectToIsVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            Type type = value.GetType();

            if (type.IsValueType)
                return true;

            if (value is string stringValue)
                return !string.IsNullOrEmpty(stringValue);

            if (type.IsArray)
                return ((Array)value).LongLength > 0;

            if (value is IEnumerable enumerable)
            {
                //True if not empty
                //This is faster than "enumerable.GetEnumerator().MoveNext()"
                foreach (object _ in enumerable)
                    return true;

                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException($"{nameof(ObjectToIsVisibleConverter)} cannot convert back. Is OneWay.");
        }
    }
}
