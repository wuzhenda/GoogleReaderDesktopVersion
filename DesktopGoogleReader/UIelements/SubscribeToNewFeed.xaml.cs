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
using System.Threading;
using System.Windows.Threading;

namespace DesktopGoogleReader.UIelements
{
    /// <summary>
    /// Interaction logic for SubscribeToNewFeed.xaml
    /// </summary>
    public partial class SubscribeToNewFeed : Window
    {
        public SubscribeToNewFeed()
        {
            InitializeComponent();
        }

        private void buttonSubscribe_Click(object sender, RoutedEventArgs e)
        {
            if (DesktopGoogleReader.AppController.CurrentReader.SubscribeToFeedUrl(textBoxFeedUrl.Text))
            {
                labelStatusText.Content = "Feed has been added successfully";
                buttonCancel.IsEnabled = false;
                buttonSubscribe.IsEnabled = false;
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
            else
            {
                labelStatusText.Content = "Can't add this feed to Google Reader";
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
