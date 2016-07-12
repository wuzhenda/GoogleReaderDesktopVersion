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
    class Posterous
    {
        static public void add(UnreadItem item)
        {
            if (item != null)
            {
                string result = HttpCommunications.SendPostRequest(@"https://posterous.com/api/newpost", new
                {
                    body = item.ItemUrl,
                    title = item.Title,
                    source = "Desktop Google Reader",
                    sourceLink = "http://desktopgooglereader.codeplex.com/"

                }, Properties.Settings.Default.posterousUser, Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.posterousPassword)), false);
                LogWriter.WriteTextToLogFile("Read It later response: " + result);
                bool success = (!string.IsNullOrEmpty(result) && result.ToLowerInvariant().Contains("<rsp stat=\"ok\">"));
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Error sending item to Posterous", "Posterous error");
                }

            }
        }
    }
}
