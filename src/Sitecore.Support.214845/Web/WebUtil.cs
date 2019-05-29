using System;
using System.Web;

namespace Sitecore.Support.Web
{
  public class WebUtil
  {
    public static string GetServerUrl()
    {
      return GetServerUrl(false);
    }

    public static string GetServerUrl(bool forcePort)
    {
      HttpContextWrapper httpContext = GetHttpContext();
      return GetServerUrl(forcePort, httpContext);
    }

    public static string GetServerUrl(bool forcePort, HttpContextBase context)
    {
      if (context != null)
      {
        return GetServerUrl(context.Request.Url, forcePort);
      }
      string serverUrl = Globals.ServerUrl;
      if (string.IsNullOrEmpty(serverUrl))
      {
        return string.Empty;
      }
      if (!forcePort)
      {
        return serverUrl;
      }
      int num = serverUrl.LastIndexOf(':');
      if (((num >= 0) && (num < (serverUrl.Length - 1))) && (serverUrl[num + 1] != '/'))
      {
        return serverUrl;
      }
      return string.Format("{0}:80", StringUtil.RemovePostfix(':', serverUrl));
    }

    public static string GetServerUrl(Uri url, bool forcePort)
    {
      if (url == null)
      {
        return string.Empty;
      }
      string scheme = url.Scheme;
      string host = url.Host;
      string str3 = url.Port.ToString();
      string str4 = string.Format("{0}://{1}", scheme, host);
      if (!forcePort)
      {
        return str4;
      }
      return (str4 + string.Format(":{0}", str3));
    }

    private static HttpContextWrapper GetHttpContext()
    {
      if (HttpContext.Current != null)
      {
        return new HttpContextWrapper(HttpContext.Current);
      }
      return null;
    }
  }
}