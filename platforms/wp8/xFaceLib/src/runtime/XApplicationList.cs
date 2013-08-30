using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xFaceLib.Log;
using Microsoft.Phone.Controls;

namespace xFaceLib.runtime
{
    public class XApplicationList
    {
        private List<XApplication> appList;

        /// <summary>
        /// 用于标识默认应用的Id
        /// </summary>
        private string defaultAppId;
        public string DefaultAppId
        {
            get { return defaultAppId; }
        }

        public XApplicationList()
        {
            appList = new List<XApplication>();
            defaultAppId = null;
        }

        /// <summary>
        /// 在已安装应用列表中添加一个应用
        /// </summary>
        /// <param name="app">待添加应用</param>
        public void Add(XApplication app)
        {
            if (null != app)
            {
                appList.Add(app);
            }
        }

        /// <summary>
        /// 根据appId获取对应的应用
        /// </summary>
        /// <param name="appId">用于获取应用的appId</param>
        /// <returns>返回获取到的应用，获取失败返回null</returns>
        public XApplication GetAppById(string appId)
        {
            if (null == appId)
            {
                XLog.WriteWarn("getAppById: app id is null!");
                return null;
            }
            foreach (XApplication app in appList)
            {
                if (app.AppInfo.AppId.Equals(appId))
                {
                    return app;
                }
            }

            XLog.WriteWarn("getAppById: Can't find app by id: " + appId);
            return null;
        }

        /// <summary>
        /// 根据appViewId获取对应的app
        /// </summary>
        /// <param name="viewId">用于获取应用的ViewId</param>
        /// <returns>返回获取到的应用，获取失败返回null</returns>
        public XApplication GetAppByViewId(string viewId)
        {
            return null;
        }

        /// <summary>
        /// 根据app view Id 获取对应的appId
        /// </summary>
        /// <param name="viewId">用于获取应用Id的ViewId</param>
        /// <returns>返回获取到的应用Id，获取失败返回null</returns>
        public string GetAppIdByViewId(string viewId)
        {
            return null;
        }

        /// <summary>
        /// 根据appId获取对应的应用
        /// </summary>
        /// <param name="browser">用于获取应用的webbrowser</param>
        /// <returns>返回获取到的应用，获取失败返回null</returns>
        public XApplication GetAppByBrowser(WebBrowser browser)
        {
            if (null == browser)
            {
                XLog.WriteWarn("GetAppByBrowser: browser is null!");
                return null;
            }
            foreach (XApplication app in appList)
            {
                //app没有绑定对应的AppView
                if (null == app.AppView)
                {
                    continue;
                }

                if (app.AppView.Browser.Equals(browser))
                {
                    return app;
                }
            }
            XLog.WriteWarn("GetAppByBrowser: Can't find app by browser!");
            return null;
        }

        /// <summary>
        /// 判断已安装的应用列表中是否存在指定的app
        /// </summary>
        /// <param name="appId">待检查应用对应的Id</param>
        /// <returns>如果已安装列表中存在指定应用返回true,否则返回false</returns>
        public bool ContainsApp(string appId)
        {
            return false;
        }

        /// <summary>
        /// 在已安装应用列表中删除指定应用
        /// </summary>
        /// <param name="appId">待删除的指定应用Id</param>
        public void RemoveAppById(string appId)
        {
            if (null == appId)
            {
                XLog.WriteWarn("removeAppById: app id is null!");
                return;
            }
            foreach (XApplication app in appList)
            {
                if (app.AppInfo.AppId.Equals(appId))
                {
                    appList.Remove(app);
                    break;
                }
            }
        }

        /// <summary>
        /// 将指定应用Id标记为默认应用Id
        /// </summary>
        /// <param name="appId">待标识为默认应用的appId</param>
        public void MarkAsDefaultApp(string appId)
        {
            defaultAppId = appId;
        }

        /// <summary>
        /// 获取默认应用
        /// </summary>
        /// <returns>返回默认应用，如果还未设置默认应用则返回null</returns>
        public XApplication GetDefaultApp()
        {
            return GetAppById(defaultAppId);
        }

        /// <summary>
        /// 获取用于迭代已安装应用列表的迭代器
        /// </summary>
        /// <param name="excludePortal">是否跳过portal</param>
        /// <returns>迭代器</returns>
        public List<XApplication>.Enumerator GetEnumerator()
        {
            return appList.GetEnumerator();
        }
    }
}
