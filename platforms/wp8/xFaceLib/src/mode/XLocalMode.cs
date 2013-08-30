using System;
using xFaceLib.runtime;
using xFaceLib.Util;

namespace xFaceLib.mode
{
    public class XLocalMode : XAppRunningMode
    {
        public XLocalMode()
        {
            this.Mode = RUNNING_MODE.LOCAL;
        }

        /// <summary>
        /// app对应的URL
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public override Uri GetURL(XApplication app)
        {
            string entry;
            if (app.AppInfo.IsAssets)
            {
                entry = XConstant.PRE_INSTALL_SOURCE_ROOT + app.AppInfo.AppId + "\\" + app.AppInfo.Entry;
            }
            else
            {
                entry = app.InstalledDirectory() + "\\" + app.AppInfo.Entry;
            }

            Uri startPage = new Uri(entry, UriKind.Relative);
            return startPage;
        }
    }
}
