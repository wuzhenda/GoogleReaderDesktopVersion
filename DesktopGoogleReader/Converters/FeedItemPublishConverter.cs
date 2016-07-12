namespace DesktopGoogleReader
{
    using System;
    using System.Windows.Data;
    using GoogleReaderAPI.DataContracts;

    [ValueConversion(typeof(string), typeof(string))]
    public class FeedItemPublishConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            UnreadItem item = value as UnreadItem;
            if (item != null)
            {
                DateTime myDateObject = new DateTime(item.Published);

                return string.Format("{0}.{1}.{2} {3}:{4}", myDateObject.Day, myDateObject.Month, myDateObject.Year, myDateObject.Hour,myDateObject.Minute);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
