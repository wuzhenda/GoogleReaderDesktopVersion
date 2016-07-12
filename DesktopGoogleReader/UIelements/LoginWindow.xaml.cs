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
using System.Security.Cryptography;

namespace DesktopGoogleReader
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        

        public LoginWindow()
        {
            InitializeComponent();
            RetrieveSettings();
        }

        #region Public API

        public enum LoginProgress 
        { 
            LoginOK,
            LoginFailed,
            LoginConnectionError,
            DataFetched
        }

        public class LoginEventArgs : EventArgs 
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public event EventHandler<LoginEventArgs> Loggining;

        public void ReportLoginProgress(LoginProgress result)
        {
            if (result == LoginProgress.LoginFailed) {
                ShowLoginFailedMessage("Login failed, try again");
            }
            else if (result == LoginProgress.LoginConnectionError)
            {
                ShowLoginFailedMessage("Connection error");
            }
            else if (result == LoginProgress.LoginOK)
            {
                ShowLoginOKMessage();
            }
        }

        #endregion



        private void ShowLoginFailedMessage(string buttonText)
        {
            btnLogin.Content = buttonText;

            AsyncHandler<bool> h = new AsyncHandler<bool>(
                (() =>
                {
                    Thread.Sleep(4000); return true;
                }),
                ((r, e) =>
                {
                    btnLogin.Content = "Login";
                    btnLogin.IsEnabled = true;
                }),
                Dispatcher);

            h.Invoke();
        }

        private void ShowLoginOKMessage()
        {
            btnLogin.Content = "Login OK, fetching data...";
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            btnLogin.Content = "Logging in...";
            if (Loggining != null) 
            {
                Loggining(this, new LoginEventArgs() 
                { 
                    Username = txtUserName.Text,
                    Password = txtPassword.Password
                });
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.LoginEmail = txtUserName.Text;
            if (chkRemember.IsChecked.GetValueOrDefault(false))
            {
                Properties.Settings.Default.LoginPassword = Crypto.EncryptString(Crypto.ToSecureString(txtPassword.Password));
                Properties.Settings.Default.RememberMe = true;
            }
            else
            {
                Properties.Settings.Default.LoginPassword = "";
                Properties.Settings.Default.RememberMe = false;
            }
            Properties.Settings.Default.Save();
        }

        private void RetrieveSettings()
        {
            txtUserName.Text = Properties.Settings.Default.LoginEmail;
            if (Properties.Settings.Default.RememberMe)
            {
                chkRemember.IsChecked = true;
                txtPassword.Password = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.LoginPassword));
                txtPassword.Focus();
            }
            else
            {
                txtUserName.Focus();
            }
            if(txtUserName.Text ==  "") {
                txtUserName.Focus();
            }
            else
            {
                txtPassword.Focus();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void chkRemember_Checked(object sender, RoutedEventArgs e)
        {
        }



        private void buttonPreferenes_Click(object sender, RoutedEventArgs e)
        {
            DesktopGoogleReader.UIelements.Preferences myPrefWindow = new DesktopGoogleReader.UIelements.Preferences();
            myPrefWindow.Show();
            myPrefWindow.Topmost = true;
        }
    }
}
