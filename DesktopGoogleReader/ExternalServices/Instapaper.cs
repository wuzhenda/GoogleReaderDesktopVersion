using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoogleReaderAPI.DataContracts;

namespace DesktopGoogleReader.ExternalServices
{
    class Instapaper
    {

        static public bool checkLoginData(string Instapaper_username, string Instapaper_password)
        {
            if (Instapaper_username != "")
            {
                string result = HttpCommunications.SendPostRequest(@"https://www.instapaper.com/api/authenticate", new
                {
                    username = Instapaper_username,
                    password = Instapaper_password
                }, false);
                LogWriter.WriteTextToLogFile("Instapaper response: " + result);
                bool success = (!string.IsNullOrEmpty(result) && result.ToLowerInvariant().StartsWith("200"));
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Invalid username or password", "Login to Instapaper failed");
                }
                return success;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Missing username", "Login to Instapaper failed");
                return false;
            }
        }

        static public void addToList(UnreadItem item)
        {
            if (item != null)
            {


                string result = HttpCommunications.SendPostRequest(@"https://www.instapaper.com/api/add", new
                {
                    username = Properties.Settings.Default.Instapaper_username,
                    password = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.Instapaper_password)),
                    url = item.ItemUrl,
                    title = item.Title

                }, false);
                LogWriter.WriteTextToLogFile("Instapaper response: " + result);
                bool success = (!string.IsNullOrEmpty(result) && result.ToLowerInvariant().StartsWith("201"));
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Error sending item to Instapaper", "Instapaper error");
                }

            }
        }

    }
}
