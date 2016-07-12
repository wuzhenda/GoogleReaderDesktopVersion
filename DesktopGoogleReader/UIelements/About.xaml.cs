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
using System.Diagnostics;

namespace DesktopGoogleReader.UIelements
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            System.Diagnostics.FileVersionInfo version = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.label_versionName.Content = version.ProductVersion;
        }

        private void button_snarlChsarp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.noer.it/download/SnarlConnector.zip");
        }

        private void button_TweetSharp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://tweetsharp.codeplex.com/");
        }

        private void button_iconSet_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://wefunction.com/2008/07/function-free-icon-set/");
        }

        private void button_facebook_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://facebooktoolkit.codeplex.com/");
        }

        private void button_awesomium_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://chriscavanagh.wordpress.com/");
        }

        private void button_winkle_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://github.com/seboslaw/Winkle");
        }


    }
}
