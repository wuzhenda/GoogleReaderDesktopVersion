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
using Dimebrain.TweetSharp.Fluent;
using Dimebrain.TweetSharp.Model;
using Dimebrain.TweetSharp.Extensions;
using DesktopGoogleReader.Properties;

namespace DesktopGoogleReader.ExternalServices
{
    /// <summary>
    /// Interaction logic for TweetIt.xaml
    /// </summary>
    public partial class TweetIt : Window
    {
        public TweetIt(UnreadItem item)
        {
            InitializeComponent();
            textBox1.Text = ExternalServices.ShortURL.ConvertURL.ShortenURL(item.ItemUrl,DesktopGoogleReader.ExternalServices.ShortURL.ShortURLProvider.Bitly) + " - " + item.Title;
            if (!CheckHasAuthorization())
            {
                PerformOAuthAuthorization();
            }       
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            label_characters.Content = (140 - textBox1.Text.Length).ToString();
        }

        private void PerformOAuthAuthorization()
        {
            var dlg = new Twitter.OAuth();
            bool? result = dlg.ShowDialog();
            if (result.HasValue == result.Value)
            {
                if (VerifyOAuthCredentials())
                {
                    /*AuthStatusLabel.Content = "Authorized";
                    AuthStatusLabel.Foreground = Brushes.Green; */
                }
            }
            else
            {
                /* AuthStatusLabel.Content = "Authorization cancelled.";
                AuthStatusLabel.Foreground = Brushes.Red;
                tryAgainButton.Visibility = Visibility.Visible; */
            }
        }

        private bool CheckHasAuthorization()
        {
            bool authorized = false;
            if (!string.IsNullOrEmpty(Settings.Default.TwitterAccessToken)
                 && !string.IsNullOrEmpty(Settings.Default.TwitterAccessTokenSecret))
            {
                authorized = VerifyOAuthCredentials();
            }
            else
            {
                /*AuthStatusLabel.Content = "Auth tokens not found.";
                AuthStatusLabel.Foreground = Brushes.Red; */
            }
            return authorized;
        }

        private static bool VerifyOAuthCredentials()
        {
            bool authorized = false;
            var twitter = FluentTwitter.CreateRequest()
                .AuthenticateWith(Settings.Default.TwitterConsumerKey, Settings.Default.TwitterConsumerSecret,
                                  Settings.Default.TwitterAccessToken, Settings.Default.TwitterAccessTokenSecret)
                .Account().VerifyCredentials();
            var response = twitter.Request();
            var profile = response.AsUser();
            if (profile != null)
            {
                authorized = true;
            }
            return authorized;
        }

        private void tryAgainBtn_Click(object sender, RoutedEventArgs e)
        {
            PerformOAuthAuthorization();
        }

        private void button_tweetIt_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var twitter = FluentTwitter.CreateRequest()
                    .AuthenticateWith(Settings.Default.TwitterConsumerKey, Settings.Default.TwitterConsumerSecret,
                                      Settings.Default.TwitterAccessToken, Settings.Default.TwitterAccessTokenSecret)
                    .Account().VerifyCredentials();

                var response = twitter.Request();
                var profile = response.AsUser();

                if (profile != null)
                {

                    var twitterPost = FluentTwitter.CreateRequest()
                    .AuthenticateWith(Settings.Default.TwitterConsumerKey, Settings.Default.TwitterConsumerSecret,
                                      Settings.Default.TwitterAccessToken, Settings.Default.TwitterAccessTokenSecret)
                                      .Statuses().Update(textBox1.Text)
                                      ;
                    TwitterResult postresponse = twitterPost.Request();
                    if (postresponse.IsTwitterError)
                    {
                        MessageBox.Show(postresponse.Response, "Error sending to Twitter", MessageBoxButton.OK);
                    }
                    else
                    {
                        this.Close();
                    }

                    
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error sending to Twitter", MessageBoxButton.OK);
            }
        }

    }
}
