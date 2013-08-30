using System;
using System.Windows;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Windows.Storage;
using xFaceLib.runtime;
using xFaceLib.Log;

namespace xFaceLib.Util
{
    public class PersistenceAppItem
    {
        public bool IsAssets { get; set; }
        public string AppId { get; set; }
    }

    public class XApplicationPersistence
    {
        private const String USER_APPS = "userApps";

        /// <summary>
        /// XApplicationPersistence构造函数
        /// </summary>
        public XApplicationPersistence()
        {

        }

        /// <summary>
        /// 从ApplicationSettings中获取所有已安装应用的相关信息,从对应的app.xml获取详细信息.
        /// </summary>
        /// <param name="appList">用于存储已安装应用相关信息的app list</param>
        /// <returns>成功读取app信息并添加到appList中时返回YES,否则返回NO</returns>
        public bool readAppsFromConfig(XApplicationList appList)
        {
            bool ret = false;

            string defaultAppid = getDefaultAppId();
            appList.MarkAsDefaultApp(defaultAppid);

            List<PersistenceAppItem> apps = new List<PersistenceAppItem>();
            if (IsolatedStorageSettings.ApplicationSettings.Contains(USER_APPS))
            {
                apps = IsolatedStorageSettings.ApplicationSettings[USER_APPS] as List<PersistenceAppItem>;
            }
            
            foreach (PersistenceAppItem persistenceapp in apps)
            {
                string appInstallDir = XSystemConfiguration.GetInstance().AppInstallationDir;
                //~\xFace\apps\appId\app.xml
                string appConfigPath = appInstallDir + persistenceapp.AppId + "\\" + "app.xml";
                ret = File.Exists(XUtils.BuildabsPathOnIsolatedStorage(appConfigPath));
                if (!ret)
                {
                    break;
                }
                XAppInfo appInfo = new XAppInfo().init(appConfigPath);
                if (null == appInfo)
                {
                    ret = false;
                    break;
                }
                else
                {
                    appInfo.IsAssets = persistenceapp.IsAssets;
                    XApplication app = XApplicationCreator.Create(appInfo);
                    appList.Add(app);
                }
            }

            return ret;
        }

        /// <summary>
        /// 将app对应的id记录到ApplicationSettings中.
        /// </summary>
        /// <param name="appId">待记录的app id</param>
        /// <param name="isassert">app的源码是否在Assert下</param>
        public void AddAppToConfig(string appId,bool isassert)
        {
            if (0 == appId.Length)
            {
                XLog.WriteWarn("Should not add app element with null app id!");
                return;
            }

            List<PersistenceAppItem> apps = new List<PersistenceAppItem>();
            if (IsolatedStorageSettings.ApplicationSettings.Contains(USER_APPS))
            {
                apps = IsolatedStorageSettings.ApplicationSettings[USER_APPS] as List<PersistenceAppItem>;
                PersistenceAppItem AppItem = new PersistenceAppItem { AppId = appId, IsAssets = isassert };
                apps.Add(AppItem);
                IsolatedStorageSettings.ApplicationSettings[USER_APPS] = apps;
            }
            else
            {
                PersistenceAppItem AppItem = new PersistenceAppItem { AppId = appId, IsAssets = isassert };
                apps.Add(AppItem);
                IsolatedStorageSettings.ApplicationSettings.Add(USER_APPS, apps);
            }

            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        /// <summary>
        /// 将指定的appId从ApplicationSettings中删除.
        /// </summary>
        /// <param name="appId">待移除的app id</param>
        public void removeAppFromConfig(string appId)
        {
            if (0 == appId.Length)
            {
                XLog.WriteWarn("Should not add app element with null app id!");
                return;
            }

            List<PersistenceAppItem> apps = new List<PersistenceAppItem>();
            if (IsolatedStorageSettings.ApplicationSettings.Contains(USER_APPS))
            {
                apps = IsolatedStorageSettings.ApplicationSettings[USER_APPS] as List<PersistenceAppItem>;
            }
            else
            {
                return;
            }

            foreach (PersistenceAppItem itemnode in apps)
            {
                string id = itemnode.AppId;
                if (id.Equals(appId))
                {
                    apps.Remove(itemnode);
                    break;
                }
            }

            //save
            IsolatedStorageSettings.ApplicationSettings[USER_APPS] = apps;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }


        /// <summary>
        /// 获取defaultAppId
        /// </summary>
        /// <returns>返回defaultAppId的值，不存在返回null</returns>
        private string getDefaultAppId()
        {
            List<PreInstalPackageItem> items = XSystemConfiguration.GetInstance().PrepackedApps;
            return items == null || items.Count == 0 ? null : items[0].PackageId;
        }
    }
}
