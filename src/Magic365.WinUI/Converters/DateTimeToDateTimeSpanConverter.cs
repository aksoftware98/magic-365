using Microsoft.UI.Xaml.Data;

namespace Magic365.WinUI.Converters
{
    public class DateTimeToDateTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var datetime = (DateTime?)value;
            if (datetime == null)
                return null;

            return new DateTimeOffset(datetime.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var timespan = (DateTimeOffset)value;

            return new DateTime(timespan.Year, timespan.Month, timespan.Day);
        }
    }
}
