using System;
using System.IO;
using System.Net;
using System.Text;

namespace DesktopGoogleReader.ExternalServices.ShortURL
{
enum ShortURLProvider
{
Bitly,
Isgd,
Tinyurl,
}

class ConvertURL
{
public static string ShortenURL(string strUrl, ShortURLProvider eService)
{
// return empty strings if not valid
if( !IsValidURL( strUrl ))
{
return "";
}

string requestUrl = string.Format(GetRequestTemplate(eService), strUrl);
WebRequest request = HttpWebRequest.Create(requestUrl);
request.Proxy = null;
string strResult = null;
try
{
using (Stream responseStream = request.GetResponse().GetResponseStream())
{
StreamReader reader = new StreamReader(responseStream, Encoding.ASCII);
strResult = reader.ReadToEnd();
if( !IsValidURL(strResult))
{
WebException w = new WebException(strResult);

throw w;
}
}
}
catch( Exception )
{
return strUrl; // eat it and return original url
}

// if converted is longer than original, return original
if ( strResult.Length > strUrl.Length)
strResult = strUrl;

return strResult;
}

/* Validate URL */
public static bool IsValidURL(string strurl)
{
// Validate the URL
if (true == strurl.ToLower().StartsWith("http://") || true == strurl.StartsWith("https://"))
{
return true;
}
return false;
}

/* Request template for URL */
private static string GetRequestTemplate(ShortURLProvider eService)
{
string strRequest = null;
switch (eService)
{
case ShortURLProvider.Isgd:
strRequest = "http://is.gd/api.php?longurl={0}";
break;
case ShortURLProvider.Bitly:
strRequest = "http://bit.ly/api?url={0}";
break;
case ShortURLProvider.Tinyurl:
strRequest = "http://tinyurl.com/api-create.php?url={0}";
break;
default:
break;
}
return strRequest;
}
}
}