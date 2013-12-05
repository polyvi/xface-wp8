using System;
using xFaceLib.runtime;
using xFaceLib.Util;
using xFaceLib.Log;

namespace xFaceLib.mode
{
    public class XAppRunningMode
    {
        /// <summary>
        /// XAPP运行模式
        /// </summary>
        public enum RUNNING_MODE : int
        {
            LOCAL = 0,
            ONLINE,
            INVALID
        };

        public const string LOCAL_RUNNING_MODE = "local";
        public const string ONLINE_RUNNING_MODE = "online";

        /// <summary>
        /// XAPP 模式类型
        /// </summary>
        public RUNNING_MODE Mode { protected set; get; }

        /// <summary>
        /// 根据配置串，创建具体的对象
        /// </summary>
        /// <param name="modeName">配置的运行模式</param>
        /// <returns></returns>
        public static XAppRunningMode CreateMode(string modeName)
        {
            if (null == modeName || modeName.Equals(LOCAL_RUNNING_MODE))
            {
                return new XLocalMode(); 
            }
            else if (modeName.Equals(ONLINE_RUNNING_MODE))
            {
                return new XOnlineMode();
            }
            XLog.WriteError("unkonw running mode： " + modeName );
            return null;
        }

        /// <summary>
        /// app对应的URL
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public virtual Uri GetURL(XApplication app)
        {
            throw new NotImplementedException("not override method exist!");
        }

        /// <summary>
        /// app的图标URL
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public string GetIconURL(XAppInfo appInfo)
        {
            string iconfile = XUtils.GenerateAppIconPath(appInfo.AppId, appInfo.Icon);
            if (null == iconfile)
                return "";
            if (appInfo.Icon.Length <= 0)
                return "";
            string abspath = XUtils.BuildabsPathOnIsolatedStorage(iconfile);
            string url = XConstant.FILE_SCHEME + abspath;
            url = url.Replace('\\', '/');
            return url;
        }
    }
}
