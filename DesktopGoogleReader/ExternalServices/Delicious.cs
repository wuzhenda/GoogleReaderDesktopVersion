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
    class Delicious
    {
        static public void add(UnreadItem item)
        {
            if (item != null)
            {
                string result = HttpCommunications.SendPostRequest(@"https://api.del.icio.us/v1/posts/add", new
                {
                    url = item.ItemUrl,
                    description = item.Title

                }, Properties.Settings.Default.deliciousUser, Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.deliciousPassword)), false);
                LogWriter.WriteTextToLogFile("Read It later response: " + result);
                bool success = (!string.IsNullOrEmpty(result) && result.ToLowerInvariant().Contains("<result code=\"done\" />"));
                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("Error sending item to del.icio.us", "del.icio.us error");
                }

            }
        }

  
    }
}
