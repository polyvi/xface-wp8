using System;
using System.IO;
using xFaceLib.runtime;
using xFaceLib.ams;
using xFaceLib.Util;

namespace xFaceLib.extensions.ams
{
    /// <summary>
    /// 为ams extension需要的ams相关功能提供实现
    /// </summary>
    public class XAmsImpl : XAms
    {
        private XAppManagement appManagement;

        public XAmsImpl(XAppManagement appManagement)
        {
            this.appManagement = appManagement;
        }

        public bool StartApp(String appId, String appparams)
        {
            return appManagement.StartApp(appId, appparams);
        }

        public void InstallApp(String path, XAppInstallListener listener)
        {
            String abspath = XUtils.BuildabsPathOnIsolatedStorage(path);
            appManagement.InstallApp(abspath, listener);
        }

        public void UpdateApp(String path, XAppInstallListener listener)
        {
            String abspath = XUtils.BuildabsPathOnIsolatedStorage(path);
            appManagement.UpdateApp(abspath, listener);
        }

        public void UninstallApp(String appId, XAppInstallListener listener)
        {
            appManagement.UninstallApp(appId, listener);
        }

        public void CloseApp(String appId)
        {
            appManagement.CloseApp(appId);
        }

        public XApplicationList GetAppList()
        {
            return appManagement.GetAppList();
        }

        public bool CanExecuteAmsBy(XApplication app)
        {
            return appManagement.IsDefaultApp(app) ? true : false;
        }

        public String[] GetPresetAppPackages()
        {
            return appManagement.GetPresetAppPackages();
        }

        public XAppInfo GetDefaultAppInfo()
        {
            return appManagement.GetAppList().GetDefaultApp().AppInfo;
        }

    }
}
