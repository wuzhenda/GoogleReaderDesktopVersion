using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GoogleReaderAPI.DataContracts;
using System.Threading;
using System.Windows.Threading;

namespace DesktopGoogleReader.UIelements
{
    /// <summary>
    /// Interaction logic for Unsubscribe.xaml
    /// </summary>
    public partial class Unsubscribe : Window
    {
        UnreadFeed actualFeed = new UnreadFeed();

        public Unsubscribe(UnreadFeed feed)
        {
            InitializeComponent();
            this.Title = "Unsubscribe from " + feed.Title;
            this.label_feedName.Content = feed.Title;
            actualFeed = feed;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_yes_Click(object sender, RoutedEventArgs e)
        {
            button_yes.Content = "Unsubscribing...";
            button_yes.IsEnabled = false;
            button1.IsEnabled = false;
            DesktopGoogleReader.AppController.Current.setFeedAsRead(actualFeed);

            if (DesktopGoogleReader.AppController.CurrentReader.UnsubscribeFromFeedUrl(actualFeed.Url))
            {
                button_yes.Content = "Success";
            }
            else
            {
                button_yes.Content = "No success";
            }

            AsyncHandler<bool> h = new AsyncHandler<bool>(
                (() =>
                {
                    Thread.Sleep(4000); return true;
                }),
                ((r, exp) =>
                {
                    this.Close();
                }),
                    Dispatcher);

            h.Invoke();
        }
    }
}
