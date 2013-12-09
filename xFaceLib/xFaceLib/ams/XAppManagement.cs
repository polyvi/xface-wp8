using System;
using System.Collections.Generic;
using xFaceLib.runtime;
using xFaceLib.Util;
using System.IO.IsolatedStorage;
using xFaceLib.Log;
using WPCordovaClassLib.Cordova.Commands;

namespace xFaceLib.ams
{
    /// <summary>
    /// 负责应用的安装、卸载、运行和更新
    /// </summary>
    public class XAppManagement
    {
        public const String kAppEventMessage = "message";
        public const String kAppEventStart = "start";
        public const String kAppEventClose = "close";

        private XAmsDelegate amsDelegate;
        /// <summary>
        /// 安装器
        /// </summary>
        private XAppInstaller appInstaller;

        /// <summary>
        /// 该用于管理已添加的app
        /// </summary>
        private List<XApplication> activeApps;

        /// <summary>
        /// 安装app应用初始化AppInstaller
        /// </summary>
        private void InitAppInstaller()
        {
            if (null == this.appInstaller)
            {
                this.appInstaller = new XAppInstaller();
            }
        }

        public XAppManagement(XAmsDelegate amsDelegate) 
        {
            this.amsDelegate = amsDelegate;
        }

        /// <summary>
        /// ams模块的初始化工作
        /// </summary>
        public void Init()
        {
            InitAppInstaller();
            activeApps = new List<XApplication>();
        }

        /// <summary>
        /// 获取应用列表
        /// </summary>
        public XApplicationList GetAppList()
        {
            return appInstaller.GetInstalledAppList();
        }

        /// <summary>
        /// 启动app应用
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="appparams">应用参数</param>
        /// <returns>启动应用的错误码</returns>
        public AMS_ERROR StartApp(String appId, XStartParams appparams)
        {
            XApplication app = this.GetAppList().GetAppById(appId);
            if (null == app)
            {
                XLog.WriteError("failed to startapp! can't find app by id:" + appId);
                return AMS_ERROR.APP_NOT_FOUND;
            }

            if (!IsValidAppEntry(app.AppInfo.Entry))
            {
                XLog.WriteError("failed to startapp! app entry can't be NullOrEmpty by id:" + appId);
                return AMS_ERROR.APP_ENTRY_ERR;
            }

            String startData = null;
            if (null != appparams)
            {
                startData = appparams.Data;
            }

            //TODO: 对于active app是否应该bringtoTop
            if (app is XNativeApplication)
            {
                //FIXME: 对于启动nativeApp 成功与否系统会做出提示
                app.Load();
            }
            else
            {
                if (!app.IsActive())
                {
                    activeApps.Add(app);
                    //注册close 和 sendMessage的event监听
                    EventHandler<string> AppCloseHandler = delegate(object o, string id)
                    {
                        CloseApp(id);
                    };
                    ((XWebApplication)app).AppClose += AppCloseHandler;

                    EventHandler<string> AppSendMsgHandler = delegate(object o, string id)
                    {
                        HandleAppEvent((XApplication)o, kAppEventMessage, id);
                    };
                    ((XWebApplication)app).AppSendMessage += AppSendMsgHandler;

                    String pageEntry = null;
                    if (null != appparams)
                    {
                        pageEntry = appparams.PageEntry;
                    }
                    if (pageEntry != null && pageEntry != string.Empty)
                    {
                        app.AppInfo.Entry = pageEntry;
                        if (IsValidAppEntry(app.AppInfo.Entry))
                        {
                            XLog.WriteError("failed to startapp! app entry can't be NullOrEmpty by id:" + appId);
                            return AMS_ERROR.APP_ENTRY_ERR;
                        }
                    }
                    if (startData != null)
                    {
                        ((XWebApplication)app).SetData(XConstant.TAG_APP_START_PARAMS, startData);
                    }
                    this.amsDelegate.StartApp((XWebApplication)app);
                }
                else
                {
                    XLog.WriteError("failed to startapp! app already running by id:" + appId);
                    return AMS_ERROR.APP_ALREADY_RUNNING;
                }
            }
            //没错误
            return AMS_ERROR.ERROR_BASE;
        }

        /// <summary>
        /// 安装app应用
        /// </summary>
        /// <param name="packagePath">应用安装包的绝对路径</param>
        public void InstallApp(String path, XInstallListener listener)
        {
            appInstaller.Install(path, listener);
        }

        /// <summary>
        /// 更新app应用
        /// </summary>
        /// <param name="packagePath">应用安装包的绝对路径</param>
        public void UpdateApp(String path, XInstallListener listener)
        {
            appInstaller.Update(path, listener);
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        /// <param name="appId">应用对应的Id</param>
        public void UninstallApp(String appId, XInstallListener listener)
        {
            appInstaller.Uninstall(appId, listener);
        }

        /// <summary>
        /// 关闭应用
        /// </summary>
        /// <param name="appId">应用对应的Id</param>
        public void CloseApp(String appId)
        {
            XApplication app = this.GetAppList().GetAppById(appId);
            if(IsDefaultApp(app))
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
                System.Windows.Application.Current.Terminate();
            }
            if (app is XWebApplication)
            {
                this.amsDelegate.CloseApp((XWebApplication)app);
                activeApps.Remove(app);
            }
        }

        /// <summary>
        ///  获取默认应用的 id
        /// </summary>
        /// <returns>默认应用的 id</returns>
        public String GetDefaultAppId()
        {
            return GetAppList().DefaultAppId;
        }

        /// <summary>
        /// 是否是默认应用
        /// </summary>
        /// <param name="app">应用</param>
        /// <returns>是默认的app返回true,否则返回false</returns>
        public bool IsDefaultApp(XApplication app)
        {
            String appId = app.AppInfo.AppId;
            String defaultAppId = GetDefaultAppId();
            return appId.Equals(defaultAppId);
        }

        /// <summary>
        /// 将appId对应的app标识为一个portal，同时写appList.xml配置文件中
        /// </summary>
        /// <param name="appId">应用对应的Id</param>
        public void MarkAsDefaultApp(String appId)
        {
            GetAppList().MarkAsDefaultApp(appId);
            appInstaller.MarkAsDefaultApp(appId);
        }

        /// <summary>
        /// 启动默认的app
        /// </summary>
        /// <param name="startParams">启动参数</param>
        public void StartDefaultApp(XStartParams startParams)
        {
            XWebApplication defaultApp = (XWebApplication)this.GetAppList().GetAppById(GetDefaultAppId());
            defaultApp.IsDefaultApp = true;
            //TODO 注册AMS 扩展
            var amsExt = WPCordovaClassLib.Cordova.CommandFactory.CreateByServiceName("AMS");
            if (amsExt != null)
            {
                amsExt.InvokeMethodNamed("init", this);
            }
            StartApp(defaultApp.AppInfo.AppId, startParams);
        }

        /// <summary>
        /// 得到当前活动的app
        /// </summary>
        /// <returns>当前活动的app</returns>
        public XApplication GetCurrActiveApp()
        {
            if (0 == activeApps.Count)
            {
                XLog.WriteDebug("The app stack is empty when getting current Active App!");
                return null;
            }
            return activeApps[activeApps.Count - 1];
        }

        /// <summary>
        /// 获取预置包
        /// </summary>
        /// <returns>返回预置包数组，每一项是预置包名</returns>
        public String[] GetPresetAppPackages()
        {
            XApplication defaultApp = GetAppList().GetDefaultApp();
            string presetPackageDir = defaultApp.GetWorkSpace() +"\\" +XConstant.PRE_SET_APP_PACKAGE_DIR_NAME;
            string absPresetSrcDir = XUtils.BuildabsPathOnIsolatedStorage(presetPackageDir);
            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoFile.DirectoryExists(presetPackageDir))
                    return null;
                return isoFile.GetFileNames(presetPackageDir + "\\" + "*");
            }
        }

        /// <summary>
        /// 关闭所有app
        /// </summary>
        public void CloseAllApp()
        {
            while (null != activeApps && activeApps.Count > 0)
            {
                XApplication app = activeApps[activeApps.Count - 1];
                CloseApp(app.AppInfo.AppId);
            }
        }

        /// <summary>
        /// 判断是否是native app的类型
        /// </summary>
        /// <param name="app">被判断的app</param>
        /// <returns>是native类型返回true,否则返回false</returns>
        bool IsNativeApp(XApplication app)
        {
            return false;
        }

        /// <summary>
        /// 判断是否是native app的类型
        /// </summary>
        /// <param name="app">被判断的app的id</param>
        /// <returns>是native类型返回true,否则返回false</returns>
        bool IsNativeApp(String appId)
        {
            XApplication app = GetAppList().GetAppById(appId);
            return IsNativeApp(app);
        }

        /// <summary>
        /// 判断压缩包是否为NativeApp的压缩包
        /// </summary>
        /// <param name="path">压缩包的路径</param>
        /// <returns>是native压缩包返回true,否则返回false</returns>
        bool IsNativePackage(String path)
        {
            return false;
        }

        /// <summary>
        /// 获得app持久化对象
        /// </summary>
        /// <returns>app持久化对象</returns>
        public XApplicationPersistence GetAppPersistence()
        {
            return appInstaller.AppPersistence;
        }

        public void HandleAppEvent(XApplication app, string eventType, string msg)
        {
            string arg = kAppEventMessage.Equals(eventType) ? string.Format("'{0}',{1}", eventType, msg) : string.Format("'{0}'", eventType);
            XApplicationList appList = GetAppList();
            XWebApplication defaultApp = (XWebApplication)appList.GetDefaultApp();
            if (eventType.Equals(kAppEventMessage))
            {
                string jsString = JsForFireAppEvent(arg);
                if (IsDefaultApp(app))
                {
                    foreach (XApplication activeApp in activeApps)
                    {
                        if (!IsDefaultApp(activeApp))
                        {
                            ((XWebApplication)activeApp).JsEvaluator.Eval(jsString);
                        }
                    }
                }
                else
                {
                    defaultApp.JsEvaluator.Eval(jsString);
                }
            }
            else if (eventType.Equals(kAppEventStart))
            {
                if (!IsDefaultApp(app))
                {
                    string jsString = JsForFireAppEvent(arg);
                    defaultApp.JsEvaluator.Eval(jsString);
                }
            }
            else if (eventType.Equals(kAppEventClose))
            {
                string jsString = JsForFireAppEvent(arg);
                defaultApp.JsEvaluator.Eval(jsString);
            }
            else
            {
                XLog.WriteWarn("unkonw app event " + eventType);
            }
        }

        private string JsForFireAppEvent(string arg)
        {
            return "(function() { \n" +
                     "try { \n" +
                         "cordova.require('com.polyvi.xface.extension.ams.app').fireAppEvent(" + arg + "); \n" +
                     "} catch (e) { \n" +
                         "console.log('exception in fireAppEvent:' + e);\n" +
                     "} \n" +
                  "})()";
        }

        private bool IsValidAppEntry(string entry)
        {
            try
            {
                Uri url = new Uri(entry, UriKind.RelativeOrAbsolute);
                return true;
            }
            catch (UriFormatException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

    }
}
