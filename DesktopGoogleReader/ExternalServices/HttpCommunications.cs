using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;


namespace DesktopGoogleReader.ExternalServices
{
    class HttpCommunications
    {
        public static string SendPostRequest(string url, object data, string basicAuthUser, string basicAuthPassword, bool allowAutoRedirect)
        {
            try
            {
                string formData = string.Empty;
                HttpCommunications.GetProperties(data).ToList().ForEach(x =>
                {
                    formData += string.Format("{0}={1}&", x.Key, x.Value);
                });
                formData = formData.TrimEnd('&');

                url = ProcessUrl(url);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(basicAuthUser+":"+basicAuthPassword)));
                request.AllowAutoRedirect = allowAutoRedirect;
                request.Accept = "*/*";
                request.UserAgent = "Desktop Google Reader (http://desktopgooglereader.codeplex.com/)";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] encodedData = new UTF8Encoding().GetBytes(formData);
                request.ContentLength = encodedData.Length;

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(encodedData, 0, encodedData.Length);
                }

                return GetResponse(request);
            }
            catch (System.Exception e)
            {
                LogWriter.WriteTextToLogFile(e);
                return "";
            }

        }

        public static string SendPostRequest(string url, object data, bool allowAutoRedirect)
        {
            try
            {
                string formData = string.Empty;
                HttpCommunications.GetProperties(data).ToList().ForEach(x =>
                {
                    formData += string.Format("{0}={1}&", x.Key, x.Value);
                });
                formData = formData.TrimEnd('&');

                url = ProcessUrl(url);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.AllowAutoRedirect = allowAutoRedirect;
                request.Accept = "*/*";
                request.UserAgent = "Desktop Google Reader (http://desktopgooglereader.codeplex.com/)";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] encodedData = new UTF8Encoding().GetBytes(formData);
                request.ContentLength = encodedData.Length;

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(encodedData, 0, encodedData.Length);
                }

                return GetResponse(request);
            }
            catch (System.Exception e)
            {
                LogWriter.WriteTextToLogFile(e);
                return "";
            }
        }

        #region Private

        private static string ProcessUrl(string url)
        {
            string quesytionMarkSymbol = "?";
            if (url.Contains(quesytionMarkSymbol))
            {
                url = url.Replace(quesytionMarkSymbol, System.Web.HttpUtility.UrlEncode(quesytionMarkSymbol));
            }
            return url;
        }

        private static string GetResponse(HttpWebRequest request)
        {

            HttpWebResponse response;
            try
            {
                HttpWebResponse responseTemp = (HttpWebResponse)request.GetResponse();
                response = responseTemp;
            }
            catch (System.Exception e)
            {
                // some proxys have problems with Continue-100 headers
                request.ProtocolVersion = HttpVersion.Version10;
                request.ServicePoint.Expect100Continue = false;
                System.Net.ServicePointManager.Expect100Continue = false;
                HttpWebResponse responseTemp = (HttpWebResponse)request.GetResponse();
                response = responseTemp;
                System.Console.WriteLine(e.Message);
            }

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();

                return result;
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetProperties(object o)
        {
            foreach (var p in o.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                yield return new KeyValuePair<string, string>(p.Name.TrimStart('_'), System.Web.HttpUtility.UrlEncode(p.GetValue(o, null).ToString()));
            }
        }

        #endregion
    }
}
