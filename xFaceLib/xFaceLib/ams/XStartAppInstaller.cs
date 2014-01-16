using System;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using xFaceLib.runtime;
using xFaceLib.Util;
using xFaceLib.Log;
using xFaceLib.toast;
using xFaceLib.Resources;

namespace xFaceLib.ams
{
    /// <summary>
    /// 负责startapp的安装
    /// </summary>
    public class XStartAppInstaller
    {
        protected XAppManagement ams;
        protected XPreInstallListener preInsallListener;

        public XStartAppInstaller(XAppManagement ams, XPreInstallListener listener)
        {
            preInsallListener = listener;
            this.ams = ams;
        }

        /// <summary>
        /// 预装启动app
        /// </summary>
        /// <returns>启动app的id, 如果失败则返回空</returns>
        public virtual String Install()
        {
            PreInstalPackageItem startAppInfo = GetStartAppItem();
            if (null == startAppInfo)
            {
                XToastPrompt.GetInstance().Toast(xFaceLibResources.No_StartApp_In_Config);
                XLog.WriteError("error config.xml");
                return null;
            }
            // 处理defaultid不一样的情况
            XAppInfo info = CheckDefaultAppId(startAppInfo);
            if (null == info)
            {
                return null;
            }
            String appDirNameInAsset = XConstant.PRE_INSTALL_SOURCE_ROOT
                    + startAppInfo.PackageId;
            XApplication app = BuildApplication(info, appDirNameInAsset);
            if (null == app) return null;
            ams.MarkAsDefaultApp(app.AppInfo.AppId);
            return app.AppInfo.AppId;
        }

        /// <summary>
        /// check defaultId 如果默认appid和已经安装的默认appid不相同 则check失败否则check成功
        /// </summary>
        /// <param name="startAppInfo">应用的信息</param>
        /// <returns></returns>
        private XAppInfo CheckDefaultAppId(PreInstalPackageItem startAppInfo)
        {
            // 解析app的app.xml
            XAppInfo info = GetInfoFromXml(startAppInfo);
            if (null == info)
            {
                XToastPrompt.GetInstance().Toast(xFaceLibResources.Read_App_Config_Error);
                return null;
            }
            String id = info.AppId;
            String defaultId = ams.GetDefaultAppId();
            if (null != defaultId && null != id && !id.Equals(defaultId))
            {
                XToastPrompt.GetInstance().Toast(xFaceLibResources.Update_DefaultApp_Id_NoMatch_Error);
                XLog.WriteError("update failure.startapp id is different.");
                return null;
            }
            return info;
        }

        private XApplication BuildApplication(XAppInfo info, String appDirNameInAsset)
        {
            XApplication buildingApp = ams.GetAppList().GetAppById(info.AppId);
            if (null != buildingApp)
            {
                buildingApp.UpdateAppInfo(info);
                buildingApp.AppInfo.IsAssets = true;
            }
            else
            {
                buildingApp = XApplicationCreator.Create(info);
                buildingApp.AppInfo.IsAssets = true;
                XApplicationList list = ams.GetAppList();
                list.Add(buildingApp);
                ams.GetAppPersistence().AddAppToConfig(info.AppId, true);
            }

            // 解析完后将app.xml copy到工作目录 以便非第一次启动的时候统一在工作目录中初始化
            String targetPath = getAppRoot(buildingApp.AppInfo.AppId)
                    + XConstant.APP_CONFIG_FILE_NAME;
            String configFile = appDirNameInAsset + "\\" + XConstant.APP_CONFIG_FILE_NAME;
            string absSourceFileName = XUtils.BuildabsPathOnInstallationFolder(configFile);
            string absdestFileName = XUtils.BuildabsPathOnIsolatedStorage(targetPath);
            try
            {
                File.Copy(absSourceFileName, absdestFileName, true);
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException || ex is FileNotFoundException|| ex is DirectoryNotFoundException ||
                    ex is ArgumentException || ex is IOException || ex is ArgumentOutOfRangeException ||
                    ex is NotSupportedException)
                {
                    XToastPrompt.GetInstance().Toast(xFaceLibResources.Copy_AppConfig_Error);
                    XLog.WriteError(string.Format("Copy xml file {0} to {1} occur Exception {2}", absSourceFileName, absdestFileName, ex.Message));
                    return null;
                }
                throw ex;
            }

            // 将内置数据解压到workspace
            String dataPackageName = GetPreinsallDataNameInWorkSpace(appDirNameInAsset);
            if (File.Exists(dataPackageName))
            {
                String path = getAppRoot(info.AppId) + XConstant.APP_WORK_DIR_NAME + "\\";
                string abspath = XUtils.BuildabsPathOnIsolatedStorage(path);
                bool flag = XUtils.unZipFile(dataPackageName, abspath);
                if (!flag)
                {
                    XLog.WriteError("unZipFile:" + dataPackageName + "To path:" + abspath + "fail");
                }
            }
            return buildingApp;
        }

        /// <summary>
        /// 获得应用工作路径
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <returns>相对路径</returns>
        private String getAppRoot(String appId)
        {
            string path = XSystemConfiguration.GetInstance().AppInstallationDir + appId
                    + "\\";
            string absPath = XUtils.BuildabsPathOnIsolatedStorage(path);
            if (!Directory.Exists(absPath))
                Directory.CreateDirectory(absPath);
            return path;
        }

        /// <summary>
        /// 获得workspace下内置的数据路径
        /// </summary>
        /// <param name="appSrcDirName"></param>
        /// <returns>内置数据的绝对路径</returns>
        private String GetPreinsallDataNameInWorkSpace(String appSrcDirName)
        {
            string path = appSrcDirName + "\\" + XConstant.APP_WORK_DIR_NAME
                    + "\\" + XConstant.APP_DATA_PACKAGE_NAME_IN_WORKSAPCE;
            return XUtils.BuildabsPathOnInstallationFolder(path);
        }

        /// <summary>
        /// 构建一个application
        /// </summary>
        /// <param name="app">应用的信息</param>
        /// <returns>application对象</returns>
        protected XApplication BuildApplication(PreInstalPackageItem app)
        {
            XAppInfo info = GetInfoFromXml(app);

            if (null == info)
            {
                XToastPrompt.GetInstance().Toast(xFaceLibResources.Read_App_Config_Error);
                XLog.WriteError("PreInstal " + app.PackageName + " app.xml error.");
                return null;
            }
            String appDirNameInAsset = XConstant.PRE_INSTALL_SOURCE_ROOT
                    + app.PackageId;
            return BuildApplication(info, appDirNameInAsset);
        }

        private PreInstalPackageItem GetStartAppItem()
        {
            // FIXME:如果没有通过其他方式指定 则默认config.xml 中的第一个为startapp
            List<PreInstalPackageItem> items = XSystemConfiguration.GetInstance().PrepackedApps;
            return items == null || items.Count == 0 ? null : items[0];
        }

        private XAppInfo GetInfoFromXml(PreInstalPackageItem AppInfo)
        {
            // 解析app的app.xml
            // app 在asset目录下
            String appDirNameInAsset = XConstant.PRE_INSTALL_SOURCE_ROOT
                    + AppInfo.PackageId;
            string configFile = appDirNameInAsset + "\\" + XConstant.APP_CONFIG_FILE_NAME;
            // 解析app的app.xml
            Stream appxmlst = null;
            try
            {
                appxmlst = Application.GetResourceStream(new Uri(configFile, UriKind.Relative)).Stream;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is IOException || ex is ArgumentNullException || ex is NullReferenceException)
                {
                    XLog.WriteError(string.Format("GetResourceStream {0} occur Exception {1}", configFile, ex.Message));
                    return null;
                }
                throw ex;
            }

            if (null == appxmlst)
            {
                return null;
            }
            XAppConfigParser appConfigParser = XAppConfigParserFactory.CreateAppConfigParser(appxmlst);
            if (null == appConfigParser)
            {
                return null;
            }
            XAppInfo info = appConfigParser.parseConfig();
            return info;
        }
    }
}