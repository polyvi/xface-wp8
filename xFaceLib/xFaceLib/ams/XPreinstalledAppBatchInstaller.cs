using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using xFaceLib.runtime;
using xFaceLib.Util;
using xFaceLib.Log;

namespace xFaceLib.ams
{
    /// <summary>
    /// 负责所有app的预装
    /// </summary>
    public class XPreinstalledAppBatchInstaller : XStartAppInstaller
    {
        public XPreinstalledAppBatchInstaller( XAppManagement ams, XPreInstallListener listener)
            :base(ams, listener)
        {            
        }

        public override String Install()
        {
            // 安装startapp
            String startAppId = base.Install();
            // 如果startapp安装失败 则不进行其他app的预装
            if (null == startAppId) {
                preInsallListener.onFailure();
                return null;
            }
            // 安装其他预装app
            List<PreInstalPackageItem> apps = GetPreInstallAppItems();
            if (apps != null)
            {
                foreach (PreInstalPackageItem app in apps)
                {
                    XApplication application = BuildApplication(app);
                    if (application != null)
                    {
                        String appId = application.AppInfo.AppId;
                        String iconname = application.AppInfo.Icon;
                        if (iconname.Length > 0)
                        {
                            String iconFile = XConstant.PRE_INSTALL_SOURCE_ROOT + appId + "\\" + iconname;
                            String absSourceFileName = XUtils.BuildabsPathOnInstallationFolder(iconFile);
                            String destFile = XUtils.GenerateAppIconPath(appId, iconname);
                            String absdestFileName = XUtils.BuildabsPathOnIsolatedStorage(destFile);
                            try
                            {
                                //处理 icon  形如image/icon.png 形式的拷贝。image 目录不存在需创建
                                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                                {
                                    string cutstr = "/";
                                    string[] dirs = iconname.Split(cutstr.ToCharArray());
                                    string appIconDirPath = XSystemConfiguration.GetInstance().AppIconsDir + appId;
                                    string path = appIconDirPath;
                                    int i = 0;
                                    while (i < dirs.Length - 1)
                                    {
                                        path = path + "\\" + dirs[i];
                                        if (!isoFile.DirectoryExists(path))
                                        {
                                            isoFile.CreateDirectory(path);
                                        }
                                        i++;
                                    }
                                }
                                File.Copy(absSourceFileName, absdestFileName, true);
                            }
                            catch (Exception ex)
                            {
                                if (ex is ObjectDisposedException || ex is FileNotFoundException || ex is DirectoryNotFoundException ||
                                    ex is ArgumentException || ex is IOException || ex is ArgumentOutOfRangeException ||
                                    ex is NotSupportedException)
                                {
                                    //icon 拷贝失败 处理为安装成功， 报警告错
                                    XLog.WriteWarn("Copy icon Error :" + ex.Message);
                                    preInsallListener.OnSuccess();
                                    return startAppId;
                                }
                                throw ex;
                            }
                        }
                    }
                }
            }
            preInsallListener.OnSuccess();
            return startAppId;
        }

        private List<PreInstalPackageItem> GetPreInstallAppItems()
        {
            List<PreInstalPackageItem> apps = new List<PreInstalPackageItem>();
            List<PreInstalPackageItem> items = XSystemConfiguration.GetInstance().PrepackedApps;
            if (null == items)
            {
                return apps;
            }
            for (int i = 1; i < items.Count; i++)
            {
                apps.Add(items[i]);
            }
            return apps;
        }
    }
}