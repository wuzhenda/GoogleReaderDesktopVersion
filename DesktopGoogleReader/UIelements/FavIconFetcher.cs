using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;

namespace DesktopGoogleReader.UIelements
{
    class FavIconFetcher
    {
        static public Image GetFavicon(string Inurl)
        {
            Uri url = new Uri(Inurl);
            string urlHost = url.Host;
            Image BookmarkIcon = null;
            if (url.HostNameType == UriHostNameType.Dns)
            {
                string iconUrl = "http://" + urlHost + "/favicon.ico";
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(iconUrl);
                    System.Net.HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    System.IO.Stream stream = response.GetResponseStream();
                    BookmarkIcon = Image.FromStream(stream);
                }
                catch
                {

                }
                return BookmarkIcon;
            }
            else
            {
                return null;
            }
        }
    }
}
