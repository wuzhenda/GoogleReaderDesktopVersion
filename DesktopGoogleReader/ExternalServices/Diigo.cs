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
    class Diigo
    {
        static public void add(UnreadItem item)
        {
            if (item != null)
            {
                string tags = "";
                if(item.UserLabels.Count > 0) {
                    foreach (string tag in item.UserLabels)
                    {
                        tags += tag + ",";
                    }
                }
                string result = HttpCommunications.SendPostRequest(@"http://api2.diigo.com/bookmarks", new
                {
                    url = item.ItemUrl,
                    title = item.Title,
                    tags = tags

                }, Properties.Settings.Default.diigoUser, Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.diigoPassword)), false);
                LogWriter.WriteTextToLogFile("Read It later response: " + result);
                bool success = (!string.IsNullOrEmpty(result) && result.ToLowerInvariant().Contains("added 1 bookmark"));
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Error sending item to Diigo", "Diigo error");
                }

            }
        }
    }
}
