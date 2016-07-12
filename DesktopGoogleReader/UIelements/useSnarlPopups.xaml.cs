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

namespace DesktopGoogleReader.UIelements
{
    /// <summary>
    /// Interaction logic for useSnarlPopups.xaml
    /// </summary>
    public partial class useSnarlPopups : Window
    {
        public useSnarlPopups()
        {
            InitializeComponent();
        }

        private void buttonShowSnarlHomepage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.fullphat.net/");
            Close();
        }

        private void buttonDismiss_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
