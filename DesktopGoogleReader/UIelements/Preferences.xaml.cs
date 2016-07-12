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
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {

        private bool isInitializing = true;

        public Preferences()
        {
            InitializeComponent();
            loadPreferences();            
            isInitializing = false;
        }

        private void loadPreferences()
        {
            sliderNotificationLength.Value = Properties.Settings.Default.MaximumNotificationLength;
            sliderPollingInterval.Value = Properties.Settings.Default.RefreshInterval;
            checkBoxNotificationsEnabled.IsChecked = Properties.Settings.Default.NotificationsEnabled;
            checkBoxMinimizeToTray.IsChecked = Properties.Settings.Default.minimizeToTray;
            checkBoxFullTextIfEmpty.IsChecked = Properties.Settings.Default.showFullIfEmpty;
            checkBoxFullTextIfTitle.IsChecked = Properties.Settings.Default.showFullIfSameAsTitle;
            checkBoxAlwaysShowFullText.IsChecked = Properties.Settings.Default.showFullAlways;

            checkBoxEnableProxy.IsChecked = Properties.Settings.Default.proxyEnabled;
            textBoxProxyServerHost.Text = Properties.Settings.Default.proxyServer;
            textBoxProxyPort.Text = Properties.Settings.Default.proxyPort.ToString();
            textBoxProxyUser.Text = Properties.Settings.Default.proxyUser;
            passwordBoxProxy.Password = Properties.Settings.Default.proxyPassword;

            checkBoxCheckForUpdates.IsChecked = Properties.Settings.Default.useAutomaticUpdate;
            checkBoxCheckForBetaUpdates.IsChecked = Properties.Settings.Default.useAutomaticUpdateBeta;

            passwordBox_RILpassword.Password = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.RILpassword));
            passwordBox_instapaperPassword.Password = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.Instapaper_password));
            passwordBoxDeliciousPassword.Password = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.deliciousPassword));
            passwordBoxPosterousPassword.Password = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.posterousPassword));
            passwordBoxDiigoPassword.Password = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.diigoPassword));
            
        }

        private void sliderPollingInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isInitializing)
            {
                return;
            }
            AppController.Current.unreadItemMonitor_SetNewPollingInterval(60 * (int)e.NewValue);
            Properties.Settings.Default.RefreshInterval = e.NewValue;
        }




        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            

            Properties.Settings.Default.Save();
                AppController.Current.setProxy();
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void sliderNotificationLength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isInitializing)
            {
                return;
            }
            Properties.Settings.Default.MaximumNotificationLength = e.NewValue;
        }

        private void checkBoxNotificationsEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }
            AppController.Current.ResumeNotifications();
            Properties.Settings.Default.NotificationsEnabled = true;

        }

        private void checkBoxNotificationsEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }
            AppController.Current.StopNotifications();
            Properties.Settings.Default.NotificationsEnabled = false;

        }

        private void checkBoxMinimizeToTray_Checked(object sender, RoutedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }
            Properties.Settings.Default.minimizeToTray = true;
        }

        private void checkBoxMinimizeToTray_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }
            Properties.Settings.Default.minimizeToTray = false;
        }

        private void checkBoxFullTextIfEmpty_Checked(object sender, RoutedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }
            Properties.Settings.Default.showFullIfEmpty = true;
        }

        private void checkBoxFullTextIfEmpty_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }
            Properties.Settings.Default.showFullIfEmpty = false;
        }


        private void checkBoxFullTextIfTitle_Checked(object sender, RoutedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }
            Properties.Settings.Default.showFullIfSameAsTitle = true;
        }


        private void checkBoxFullTextIfTitle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }
            Properties.Settings.Default.showFullIfSameAsTitle = false;
        }

        private void checkBoxEnableProxy_Checked(object sender, RoutedEventArgs e)
        {

            textBoxProxyPort.IsEnabled = true;
            textBoxProxyServerHost.IsEnabled = true;
            textBoxProxyUser.IsEnabled = true;
            passwordBoxProxy.IsEnabled = true;

            textBoxProxyServerHost.Focus();

           Properties.Settings.Default.proxyEnabled = true;
        }

        private void checkBoxEnableProxy_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxProxyPort.IsEnabled = false;
            textBoxProxyServerHost.IsEnabled = false;
            textBoxProxyUser.IsEnabled = false;
            passwordBoxProxy.IsEnabled = false;

            Properties.Settings.Default.proxyEnabled = false;
        }

        private void textBoxProxyServerHost_TextChanged(object sender, TextChangedEventArgs e)
        {
            var thisTextBox = sender as System.Windows.Controls.TextBox;
            Properties.Settings.Default.proxyServer = thisTextBox.Text;
        }

        private void textBoxProxyUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            var thisTextBox = sender as System.Windows.Controls.TextBox;
            Properties.Settings.Default.proxyUser = thisTextBox.Text;
        }

        private void passwordBoxProxy_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var thisTextBox = sender as System.Windows.Controls.PasswordBox;
            Properties.Settings.Default.proxyPassword = thisTextBox.Password;
        }

        private void textBoxProxyPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            var thisTextBox = sender as System.Windows.Controls.TextBox;
            if (thisTextBox.Text != "")
            {
                try
                {
                    Properties.Settings.Default.proxyPort = Convert.ToInt32(thisTextBox.Text);
                    textBoxProxyPort.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                    textBoxProxyPort.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 150, 150));
                }
            }
        }

        private void buttonCheckNowForUpdate_Click(object sender, RoutedEventArgs e)
        {
            Winkle.VersionCheck myUpdateChecker = new Winkle.VersionCheck("Desktop Google Reader", "http://tlhan-ghun.de/files/desktopGoogleReaderWinkle.xml");
            myUpdateChecker.checkForUpdate(System.Reflection.Assembly.GetExecutingAssembly(), Properties.Settings.Default.useAutomaticUpdateBeta);

        }

        private void checkBoxAlwaysShowFullText_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxFullTextIfEmpty.IsEnabled = false;
            checkBoxFullTextIfTitle.IsEnabled = false;
            Properties.Settings.Default.showFullAlways = true;
        }

        private void checkBoxAlwaysShowFullText_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxFullTextIfEmpty.IsEnabled = true;
            checkBoxFullTextIfTitle.IsEnabled = true;
            Properties.Settings.Default.showFullAlways = false;
        }

 

        private void checkBoxCheckForUpdates_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.useAutomaticUpdate = true;
            if (checkBoxCheckForBetaUpdates != null)
            {
                checkBoxCheckForBetaUpdates.IsEnabled = true;
            }
        }

        private void checkBoxCheckForUpdates_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.useAutomaticUpdate = false;
            if (checkBoxCheckForBetaUpdates != null)
            {
                checkBoxCheckForBetaUpdates.IsEnabled = false;
            }
        }

        private void passwordBox_RILpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RILpassword = Crypto.EncryptString(Crypto.ToSecureString(passwordBox_RILpassword.Password));
        }

        private void button_RILTest_Click(object sender, RoutedEventArgs e)
        {
            if (ExternalServices.ReadItLater.checkLoginData(textBox_RILusername.Text, passwordBox_RILpassword.Password))
            {
                button_RILTest.Content = "Login valid";
            }
            else
            {
                button_RILTest.Content = "Test again";
            }
        }


        private void passwordBox_instapaperPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Instapaper_password = Crypto.EncryptString(Crypto.ToSecureString(passwordBox_instapaperPassword.Password));
        }

        private void button_testLoginInstapaper_Click(object sender, RoutedEventArgs e)
        {
            if (ExternalServices.Instapaper.checkLoginData(textBox_instapaperUsername.Text, passwordBox_instapaperPassword.Password))
            {
                button_testLoginInstapaper.Content = "Login valid";
            }
            else
            {
                button_testLoginInstapaper.Content = "Test again";
            }
        }

        private void passwordBoxDeliciousPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.deliciousPassword = Crypto.EncryptString(Crypto.ToSecureString(passwordBoxDeliciousPassword.Password));
        }

        private void passwordBoxPosterousPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.posterousPassword = Crypto.EncryptString(Crypto.ToSecureString(passwordBoxPosterousPassword.Password));
        }

        private void passwordBoxDiigoPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.diigoPassword = Crypto.EncryptString(Crypto.ToSecureString(passwordBoxDiigoPassword.Password));
        }



    }
}
