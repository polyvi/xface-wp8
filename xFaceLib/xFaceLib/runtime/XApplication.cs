using System.IO.IsolatedStorage;
using xFaceLib.mode;

namespace xFaceLib.runtime
{
    public class XApplication
    {
        /// <summary>
        /// app 对应的执行应用View
        /// </summary>
        private XAppWebView appView;
        public XAppWebView AppView
        {
            get { return appView; }
            set { appView = value; }
        }

        /// <summary>
        /// app相关的配置信息
        /// </summary>
        private XAppInfo appInfo;
        public XAppInfo AppInfo
        {
            get { return appInfo; }
        }

        private bool isDefaultApp;
        public bool IsDefaultApp
        {
            get { return isDefaultApp; }
            set { isDefaultApp = value; }
        }
        /// <summary>
        /// webApp的运行模式local/online
        /// </summary>
        public XAppRunningMode mode;

        /// <summary>
        /// app 启动参数
        /// </summary>
        public string startParams = "";

        public XApplication(XAppInfo applicationInfo)
        {
            this.appView = null;
            this.appInfo = applicationInfo;
            this.isDefaultApp = false;
            this.mode = XAppRunningMode.CreateMode(applicationInfo.RunningMode);
        }

        /// <summary>
        /// 获取app的工作空间，不存在时相应的创建(相对于独立存储的相对路径)
        /// 路径形如：~/xFace/apps/appId/workspace
        /// </summary>
        /// <returns>app的工作空间相对路径</returns>
        public string GetWorkSpace()
        {
            string appInstallDir = InstalledDirectory();
            string workSpace = appInstallDir + "\\" + "workspace";
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStorage.DirectoryExists(workSpace))
                {
                    isoStorage.CreateDirectory(workSpace);
                }
            }
            
            return workSpace;
        } 

        /// <summary>
        /// 获取应用存放数据的目录(相对于独立存储的相对路径)
        /// 路径形如：~/xFace/apps/appId/data
        /// </summary>
        /// <returns>返回appData目录相对路径</returns>
        public string GetDataDir()
        {
            string appInstallDir = InstalledDirectory();
            string dataDir = appInstallDir + "\\" + "data";
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStorage.DirectoryExists(dataDir))
                {
                    isoStorage.CreateDirectory(dataDir);
                }
            }
            return dataDir;
        }

        /// <summary>
        /// 应用安装目录所在的路径(相对于独立存储的相对路径)
        /// 路径形如：~/xFace/apps/appId
        /// </summary>
        /// <returns>返回app的安装目录相对路径</returns>
        public string InstalledDirectory()
        {
            string appId = this.appInfo.AppId;
            string installDir = XSystemConfiguration.GetInstance().AppInstallationDir + appId;
            return installDir;
        }

        /// <summary>
        /// 判断当前应用是否处于活动状态
        /// </summary>
        /// <returns>是活的状态返回true，否则返回false</returns>
        public bool IsActive()
        {
            return (null != this.appView);
        }

        /// <summary>
        /// app 加载应用启动
        /// </summary>
        public virtual void Load()
        {
        }


        /// <summary>
        /// app 关闭 清理已注册的callback
        /// </summary>
        public virtual void CloseApp()
        {
        }

        #region XApplication EventHandle

        /// <summary>
        /// 通知扩展此app已经卸载
        /// </summary>
        public virtual void OnAppUninstalled()
        {
        }

        #endregion

        /// <summary>
        /// 设置应用配置信息
        /// </summary>
        /// <param name="appInfo">应用配置信息</param>
        public void UpdateAppInfo(XAppInfo appInfo)
        {
            this.appInfo = appInfo;
        }

        /// <summary>
        /// 获取app的图片url
        /// </summary>
        /// <returns>图片url</returns>
        public string GetAppIconUrl()
        {
            return this.mode.GetIconURL(appInfo);
        }

        public XAppRunningMode.RUNNING_MODE GetRunningMode()
        {
            return this.mode.Mode;
        }

        public bool IsNativeApp()
        {
            return (AppInfo.Type.Equals("napp"));
        }
    }
}
