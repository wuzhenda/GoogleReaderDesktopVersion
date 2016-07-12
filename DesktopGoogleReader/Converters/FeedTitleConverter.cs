namespace DesktopGoogleReader
{
    using System;
    using System.Windows.Data;
    using GoogleReaderAPI.DataContracts;

    [ValueConversion(typeof(string), typeof(string))]
    public class FeedTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            UnreadFeed feed = value as UnreadFeed;
            if (feed != null)
            {
                return string.Format("{0} ({1})", feed.Title, feed.UnreadCount);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
