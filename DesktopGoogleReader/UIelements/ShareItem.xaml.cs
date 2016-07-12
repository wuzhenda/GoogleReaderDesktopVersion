﻿using System;
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
    /// Interaction logic for ShareItem.xaml
    /// </summary>
    public partial class ShareItem : Window
    {

        private UnreadItem item = new UnreadItem();

        public ShareItem(UnreadItem currentItem)
        {
            InitializeComponent();
            this.label_feedName.Content = currentItem.ParentFeedTitle;
            this.label_itemTitle.Content = currentItem.Title;

            item = currentItem;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_yes_Click(object sender, RoutedEventArgs e)
        {
            button_yes.Content = "Sharing...";
            button_yes.IsEnabled = false;
            button_cancel.IsEnabled = false;
            

            if (AppController.CurrentReader.SetAsShared(item.Shared, item.Id, item.ParentFeedUrl, textBox_comment.Text))
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
