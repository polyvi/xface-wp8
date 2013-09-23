using System;
using xFaceLib.runtime;
using xFaceLib.Util;

namespace xFaceLib.mode
{
    public class XOnlineMode : XAppRunningMode
    {
        public XOnlineMode()
        {
            this.Mode = RUNNING_MODE.ONLINE;
        }

        private const string PLATFORM_QUERYSTRING = "platform=wp";

        public override Uri GetURL(XApplication app)
        {
            //不含 http:// 或https://协议的 默认拼接 HTTP：//
            string url = app.AppInfo.Entry;
            url = url.ToLower();
            if (url.IndexOf("?") > 0)
            {
                url = url + "&" + PLATFORM_QUERYSTRING;
            }
            else
            {
                url = url + "?" + PLATFORM_QUERYSTRING;
            }

            if (!url.StartsWith(XConstant.HTTP_SCHEME) && !url.StartsWith(XConstant.HTTPS_SCHEME))
            {
                url = XConstant.HTTP_SCHEME + url;
            }
            return new Uri(url, UriKind.Absolute);
        }
    }
}
