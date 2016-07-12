namespace DesktopGoogleReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Data;

    [ValueConversion(typeof(string), typeof(string))]
    public class FeedItemSummarySourceConverter : IValueConverter
    {
        private static List<string> tempFilesList = new List<string>();

        public static void DeleteTempFiles()
        {
            foreach (var f in tempFilesList)
            {
                try
                {
                    File.Delete(f);
                }
                catch (Exception) { }
            }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var currentItem = value as GoogleReaderAPI.DataContracts.UnreadItem;

                if (((currentItem.Summary == "" || currentItem.Summary == string.Empty) && Properties.Settings.Default.showFullIfEmpty) || (currentItem.Title.ToLower().Trim() == currentItem.Summary.ToLower().Trim() && Properties.Settings.Default.showFullIfSameAsTitle))
                {
                    return currentItem.ItemUrl;
                }
                else
                {
                    string temp_file = Path.GetTempFileName();
                    FileInfo f = new FileInfo(temp_file);

                    temp_file = string.Format("{0}.html", temp_file.Substring(0, temp_file.LastIndexOf('.')));

                    if (!File.Exists(temp_file))
                        f.MoveTo(temp_file);

                    tempFilesList.Add(temp_file);

                    File.WriteAllText(temp_file, "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"></head><body>"
                        + currentItem.Summary
                        + "<p /><p><a style=\"border:1px solid black;padding:0.5em;background-color:#eee;float:right\" href=\""
                        + currentItem.ItemUrl
                        + "\">Read full article</a></p></body></html>");

                    return temp_file;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Empty;
        }

        #endregion
    }
}
