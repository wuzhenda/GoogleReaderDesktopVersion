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
    /// Interaction logic for EditTags.xaml
    /// </summary>
    public partial class EditTags : Window
    {
        private UnreadItem item = new UnreadItem();

        public EditTags(UnreadItem currentItem)
        {
            InitializeComponent();
            this.label_feedName.Content = currentItem.ParentFeedTitle;
            this.label_itemTitle.Content = currentItem.Title;
            foreach (string tag in currentItem.UserLabels)
            {
                this.textBox_tags.Text += tag;
                this.textBox_tags.Text += ",";
            }
            this.textBox_tags.Text = this.textBox_tags.Text.TrimEnd(',');
            item = currentItem;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_yes_Click(object sender, RoutedEventArgs e)
        {
            button_yes.Content = "Saving tags...";
            button_yes.IsEnabled = false;
            button_cancel.IsEnabled = false;


            if (AppController.CurrentReader.SetTags(item.Id, item.ParentFeedUrl, textBox_tags.Text))
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
