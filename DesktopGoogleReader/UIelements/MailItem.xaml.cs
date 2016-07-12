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

namespace DesktopGoogleReader.UIelements
{
    /// <summary>
    /// Interaction logic for MailItem.xaml
    /// </summary>
    public partial class MailItem : Window
    {
        string itemId = "";

        public MailItem(UnreadItem item)
        {
            InitializeComponent();
            label_feedName.Content = item.ParentFeedTitle;
            label_itemTitle.Content = item.Title;
            itemId = item.Id;
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_send_Click(object sender, RoutedEventArgs e)
        {
            AppController.CurrentReader.SendAsEMail(itemId,textBox_mailReceiver.Text,textBox_mailBody.Text,textBox_mailSubject.Text,(bool)checkBox_mailMeCcc.IsChecked);
            Close();
        }

        private void textBox_mailReceiver_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox_mailReceiver != null && button_send != null)
            {
                if (textBox_mailReceiver.Text != "")
                {
                    button_send.IsEnabled = true;
                }
                else
                {
                    button_send.IsEnabled = false;
                }
            }
        }
    }
}
