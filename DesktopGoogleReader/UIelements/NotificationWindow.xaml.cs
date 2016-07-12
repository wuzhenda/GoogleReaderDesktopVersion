using System.Windows;
using GoogleReaderAPI.DataContracts;

namespace DesktopGoogleReader
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : NotificationWindowBase
    {
        public NotificationWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            UnreadItem item = DataContext as UnreadItem;
            ReaderCommands.OpenPage(item);
        }
    }
}
