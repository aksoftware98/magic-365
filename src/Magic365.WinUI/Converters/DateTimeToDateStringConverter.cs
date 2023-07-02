using Microsoft.UI.Xaml.Data;

namespace Magic365.WinUI.Converters
{
    public class DateTimeToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timeValue = (DateTime?)value;

            return timeValue?.ToString("yyyy MMM dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
