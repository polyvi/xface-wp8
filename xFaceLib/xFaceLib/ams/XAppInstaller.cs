using System;
using System.Threading.Tasks;
using System.IO;
using xFaceLib.runtime;
using xFaceLib.Util;
using xFaceLib.Log;

namespace xFaceLib.ams
{
    /// <summary>
    /// 负责应用安装
    /// </summary>
    public class XAppInstaller
    {
        /// <summary>
        /// 对所有app的引用
        /// </summary>
        private XApplicationList appList;

        /// <summary>
        /// 持久化app的对象
        /// </summary>
        private XApplicationPersistence appPersistence;
        public XApplicationPersistence AppPersistence
        {
            get
            {
                return appPersistence;
            }
        }

        public XAppInstaller()
        {
            this.appList = new XApplicationList();
            this.appPersistence = new XApplicationPersistence();
            this.appPersistence.readAppsFromConfig(appList);
        }

        /// <summary>
        /// 获取installer安装的应用列表
        /// </summary>
        /// <returns>应用列表</returns>
        public XApplicationList GetInstalledAppList()
        {
            return appList;
        }

        /// <summary>
        /// 安装app应用
        /// </summary>
        /// <param name="packagePath">安装包的绝对路径</param>
        public void Install(String packagePath, XInstallListener listener)
        {
            // 安装包不存在
            if (!File.Exists(packagePath))
            {
                //安装包不存在, 通知错误信息
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL, "noId",
                   AMS_ERROR.NO_SRC_PACKAGE);
                XLog.WriteDebug("package is not exsit!");
                return;
            }
            listener.OnProgressUpdated(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL,
                InstallStatus.INSTALL_INITIALIZE);
            //获取应用安装包配置文件中的应用id
            XAppInfo appInfo = XUtils.GetAppInfoFromAppPackage(packagePath);
            if (appInfo == null)
            {
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL, "noId",
                    AMS_ERROR.NO_APP_CONFIG_FILE);
                XLog.WriteDebug("invalid package！");
                return;
            }
            String appId = appInfo.AppId;

            // 该app已经安装过
            if (appList.GetAppById(appId) != null)
            {
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL, appId,
                    AMS_ERROR.APP_ALREADY_EXISTED);
                XLog.WriteInfo("App (id=" + appId + ") has existed!");
                return;
            }
            listener.OnProgressUpdated(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL,
                InstallStatus.INSTALL_UNZIP_PACKAGE);

            // 解压应用安装包
            String dstPath = BuildPathForApp(appId);
            bool successed = XUtils.unZipFile(packagePath, dstPath);
            if (!successed)
            {
                //如果解压失败 则删除中间文件
                XUtils.DeleteFileRecursively(dstPath);
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL, appId,
                    AMS_ERROR.IO_ERROR);
                XLog.WriteDebug("unzip package failure!");
                return;
            }

            //更新已安装列表以及系统配置文件
            appInfo.IsAssets = false;
            XApplication app = XApplicationCreator.Create(appInfo);
            appList.Add(app);
            listener.OnProgressUpdated(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL,
                InstallStatus.INSTALL_WRITE_CONFIGURATION);
            appPersistence.AddAppToConfig(appId,false);

            //移动应用的icon到workdir目录下的XApplication#APPS_ICON_DIR_NAME/$appId目录中，便于defaultApp
            // 访问所有应用的icon
            if (appInfo.Icon.Length > 0)
            {
                String appDirPath = XSystemConfiguration.GetInstance().AppInstallationDir + appId;
                String iconFile = appDirPath + "\\" + appInfo.Icon;
                String destFile = XUtils.GenerateAppIconPath(appId, appInfo.Icon);
                XUtils.Move(iconFile, destFile);
            }

            //xface.js文件内置
            //online app不需要，只有local app才处理,默认为local
            if (string.IsNullOrEmpty(appInfo.RunningMode) || appInfo.RunningMode.Equals(xFaceLib.mode.XAppRunningMode.LOCAL_RUNNING_MODE))
            {
                CopyEmbeddedJsFileToApp(appId);
            }

            File.Delete(packagePath);
            //上报成功事件
            listener.OnProgressUpdated(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL,
                InstallStatus.INSTALL_FINISHED);
            listener.OnSuccess(AMS_OPERATION_TYPE.OPERATION_TYPE_INSTALL, appId);
        }

        /// <summary>
        /// 更新一个应用.只有在应用已经安装过并且新的应用安装包版本号更大的情况下才会进行更新操作
        /// </summary>
        /// <param name="packagePath">安装包的绝对路径</param>
        public void Update(String packagePath, XInstallListener listener)
        {
            // 安装包不存在
            if (!File.Exists(packagePath))
            {
                //通知错误信息
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE, "noId",
                    AMS_ERROR.NO_SRC_PACKAGE);
                XLog.WriteDebug("package is not exsit!");
                return;
            }
            listener.OnProgressUpdated(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE,
                InstallStatus.INSTALL_INITIALIZE);

            //获取应用安装包配置文件中的应用id
            XAppInfo appInfo = XUtils.GetAppInfoFromAppPackage(packagePath);
            if (appInfo == null)
            {
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE, "noId",
                    AMS_ERROR.NO_APP_CONFIG_FILE);
                XLog.WriteDebug("invalid package！");
                return;
            }
            String appId = appInfo.AppId;

            // 只能更新已经安装的应用
            XApplication oldApp = appList.GetAppById(appId);
            if (null == oldApp)
            {
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE, appId,
                    AMS_ERROR.NO_TARGET_APP);
                return;
            }
            listener.OnProgressUpdated(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE,
                InstallStatus.INSTALL_UNZIP_PACKAGE);

            //解压应用更新包
            String tempFileDir = XUtils.CreateTempDir(XSystemConfiguration.GetInstance().AppInstallationDir);
            string abstempPath = XUtils.BuildabsPathOnIsolatedStorage(tempFileDir);
            bool successed = XUtils.unZipFile(packagePath, abstempPath);

            listener.OnProgressUpdated(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE,
                InstallStatus.INSTALL_WRITE_CONFIGURATION);
            if (!successed)
            {
                XUtils.DeleteFileRecursively(abstempPath);
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE, appId,
                        AMS_ERROR.IO_ERROR);
                XLog.WriteDebug("unzip package failure!");
                return;
            }
            // 将临时目录的文件 拷贝到安装目录
            String appDirPath = XSystemConfiguration.GetInstance().AppInstallationDir + appId;
            XUtils.Copy(tempFileDir, appDirPath);
            XUtils.DeleteFileRecursively(abstempPath);

            if (appInfo.Icon.Length > 0)
            {
                // 删除XApplication#APPS_ICON_DIR_NAME下的appId目录
                String iconPath = XUtils.GenerateAppIconPath(appId, "");
                string absiconPath = XUtils.BuildabsPathOnIsolatedStorage(iconPath);
                XUtils.DeleteFileRecursively(absiconPath);
                //移动应用的icon到workdir目录下的XApplication#APPS_ICON_DIR_NAME/$appId目录中，便于defaultApp
                // 访问所有应用的icon
                String iconFile = appDirPath + "\\" + appInfo.Icon;
                String destFile = XUtils.GenerateAppIconPath(appId, appInfo.Icon);
                XUtils.Move(iconFile, destFile);
            }            

            //xface.js文件内置
            CopyEmbeddedJsFileToApp(appId);

            //更新app中的应用配置信息
            appInfo.IsAssets = false;
            oldApp.UpdateAppInfo(appInfo);

            File.Delete(packagePath);
            //上报成功事件
            listener.OnProgressUpdated(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE,
                InstallStatus.INSTALL_FINISHED);
            listener.OnSuccess(AMS_OPERATION_TYPE.OPERATION_TYPE_UPDATE, appId);
        }

        /// <summary>
        /// 卸载app应用
        /// </summary>
        /// <param name="appId">需要卸载的appid</param>
        public void Uninstall(String appId, XInstallListener listener)
        {
            XApplication app = appList.GetAppById(appId);
            if (null == app)
            {
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_UNINSTALL,
                    appId, AMS_ERROR.NO_TARGET_APP);
                return;
            }

            //删除app本地目录
            String dstPath = app.InstalledDirectory();
            string absdstPath = XUtils.BuildabsPathOnIsolatedStorage(dstPath);
            if (!Directory.Exists(absdstPath))
            {
                listener.OnError(AMS_OPERATION_TYPE.OPERATION_TYPE_UNINSTALL,
                        appId, AMS_ERROR.IO_ERROR);
                return;
            }
            XUtils.DeleteFileRecursively(absdstPath);

            //更新已安装列表以及系统配置文件
            appList.RemoveAppById(appId);
            appPersistence.removeAppFromConfig(appId);

            //删除XApplication#APPS_ICON_DIR_NAME下的appId目录
            String iconPath = XUtils.GenerateAppIconPath(appId, "");
            string absiconPath = XUtils.BuildabsPathOnIsolatedStorage(iconPath);
            XUtils.DeleteFileRecursively(absiconPath);

            //通知扩展此app已经卸载
            app.OnAppUninstalled();

            //上报成功事件
            listener.OnSuccess(AMS_OPERATION_TYPE.OPERATION_TYPE_UNINSTALL, appId);
        }

        /// <summary>
        /// 在配置文件中将appId标识为默认app的id
        /// </summary>
        /// <param name="appId">应用的id</param>
        public void MarkAsDefaultApp(String appId)
        {
        }

        /// <summary>
        /// 将内置的xface.js拷贝指定应用的根目录下
        /// </summary>
        /// <param name="appId">xface.js要拷贝到的目标应用的id</param>
        private void CopyEmbeddedJsFileToApp(String appId)
        {
            XApplication app = appList.GetAppById(appId);
            if (null == app)
            {
                return;
            }

            string apppath = XSystemConfiguration.GetInstance().AppInstallationDir + appId;
            apppath += "\\" + app.AppInfo.Entry;

            string path = Path.GetDirectoryName(apppath);
            XUtils.copyEmbeddedJsFile(path); 
        }

        /// <summary>
        /// 根据appid生成工作路径
        /// </summary>
        /// <param name="appId">应用的id</param>
        /// <returns>应用的绝对工作路径</returns>
        private String BuildPathForApp(String appId)
        {
            String path = XSystemConfiguration.GetInstance().AppInstallationDir;
            path += appId + "\\";
            string abspath = XUtils.BuildabsPathOnIsolatedStorage(path);
            return abspath;
        }
    }
}
