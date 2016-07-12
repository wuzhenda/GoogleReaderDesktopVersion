using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoogleReaderAPI;
using GoogleReaderAPI.DataContracts;
using System.Security;
using System.IO;
using System.Net;
using System.Reflection;

namespace DesktopGoogleReader.ExternalServices
{
    class ReadItLater
    {
        static private string apiKey = "a8ep6Td0Ac955d2d1agPH09GaMd5Od0x";

        static public void addToList(UnreadItem item)
        {
            if (item != null)
            {
                string result = HttpCommunications.SendPostRequest(@"https://readitlaterlist.com/v2/add", new
                {
                    username = Properties.Settings.Default.RILusername,
                    password = Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.RILpassword)),
                    apikey = apiKey,
                    url = item.ItemUrl,
                    title = item.Title

                }, false);
                LogWriter.WriteTextToLogFile("Read It later response: " + result);
                bool success = (!string.IsNullOrEmpty(result) && result.ToLowerInvariant() == "200 ok");
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Error sending item to Read It Later", "Read it later error");
                }

            }
        }

        static public bool checkLoginData(string RIL_username, string RIL_password)
        {
            if (RIL_username != "" && RIL_password != "")
            {


                string result = HttpCommunications.SendPostRequest(@"https://readitlaterlist.com/v2/auth", new
                {
                    username = RIL_username,
                    password = RIL_password,
                    apikey = apiKey,

                }, false);
                LogWriter.WriteTextToLogFile("Read It Later response: " + result);
                bool success = (!string.IsNullOrEmpty(result) && result.ToLowerInvariant() == "200 ok");
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Invalid username or password", "Login to Read It Later failed");
                }
                return success;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Missing username or password", "Login to Read It Later failed");
                return false;
            }
        }
    }
}
