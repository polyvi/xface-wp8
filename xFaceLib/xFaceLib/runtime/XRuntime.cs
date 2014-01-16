using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using xFaceLib.ams;
using xFaceLib.Util;
using xFaceLib.Log;
using xFaceLib.toast;
using xFaceLib.Resources;

namespace xFaceLib.runtime
{
    public class XRuntime : XAmsDelegate
    {
        /// <summary>
        /// page上用于布局控件的容器
        /// </summary>
        private readonly Grid layoutRoot;

        /// <summary>
        /// app应用视图控制器
        /// </summary>
        private XAppController appController;

        private XSystemBootstrap xFaceBoot;

        /// <summary>
        /// 应用管理器
        /// </summary>
        private XAppManagement appManagement;
        public XAppManagement AppManagement
        { get { return appManagement; } }

        public XRuntime(Grid layoutRoot)
        {
            this.layoutRoot = layoutRoot;
            StartupMode mode = PhoneApplicationService.Current.StartupMode;
            var umengExt = WPCordovaClassLib.Cordova.CommandFactory.CreateByServiceName("XUmengExt");
            //if resume:do nothing
            if (mode == StartupMode.Launch)
            {
                PhoneApplicationService service = PhoneApplicationService.Current;
                service.Activated += new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(AppActivated);
                service.Launching += new EventHandler<LaunchingEventArgs>(AppLaunching);
                service.Deactivated += new EventHandler<DeactivatedEventArgs>(AppDeactivated);
                service.Closing += new EventHandler<ClosingEventArgs>(AppClosing);
            }

            this.appController = new XAppController(layoutRoot);
        }

        public void PageLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PhoneApplicationPage page = (PhoneApplicationPage)sender;
            page.Loaded -= PageLoaded;
            XLog.WriteDebug("xFacePage.Width = " + page.ActualWidth.ToString());
            XLog.WriteDebug("xFacePage.Height = " + page.ActualHeight.ToString());
            Thread thread = new Thread(new ThreadStart(doInitialization));
            thread.Start();
        }

        /// <summary>
        /// 进行runtime的初始化，然后启动应用
        /// </summary>
        private  void doInitialization()
        {
            //异步系统环境准备
             Task.Run(() =>
            {
                this.xFaceBoot = XSystemBootstrapFactory.CreateSystemBootstrap();
                this.xFaceBoot.AddVersionLabel(this.layoutRoot);
                this.xFaceBoot.FinishToPrepareWorkEnvironment += FinishToPrepareWorkEnvironment;
                this.xFaceBoot.FailToPrepareWorkEnvironment += FailToPrepareWorkEnvironment;
                xFaceBoot.PrepareWorkEnvironment();
            });
        }

        private void FinishToPrepareWorkEnvironment(object sender, string reult)
        {
            initialize();
        }

        private void FailToPrepareWorkEnvironment(object sender, string reult)
        {
            XToastPrompt.GetInstance().Toast(xFaceLibResources.System_Initialize_Error);
        }

        /// <summary>
        /// 管理器的初始化
        /// </summary>
        private void initialize()
        {
            //管理器的初始化
            if (null == appManagement)
            {
                appManagement = new XAppManagement(this);
                appManagement.Init();
            }

            //初始化完成，启动default app
            //startDefaultApp
            this.xFaceBoot.Boot(appManagement);
        }

        private void CheckVersion(XApplication app)
        {
            //FIXME: 暂不处理 EngineVersion 无值的判断
            if (null != app.AppInfo.EngineVersion)
            {
                try
                {
                    Version appEngineVersion = new Version(app.AppInfo.EngineVersion);
                    Version sysEngineVersion = new Version(XSystemConfiguration.GetInstance().XFaceVersion);

                    if (appEngineVersion > sysEngineVersion)
                    {
                        XToastPrompt.GetInstance().Toast(xFaceLibResources.Engine_Version_TooLow_Error);
                    }
                }
                catch (FormatException ex)
                {
                    XLog.WriteError("CheckVersion error:" + ex.Message);
                }
            }
        }

        #region XAmsDelegate impl

        /// <summary>
        /// 启动application
        /// </summary>
        /// <param name="app">待启动的application</param>
        public void StartApp(XWebApplication app)
        {
            CheckVersion(app);
            this.appController.CreateView(app);
            //app start 事件的 js eval
            this.appManagement.HandleAppEvent(app, XAppManagement.kAppEventStart, "");
        }

        /// <summary>
        /// 关闭application
        /// </summary>
        /// <param name="app">待关闭的application</param>
        public void CloseApp(XWebApplication app)
        {
            appController.CloseView(app);
            app.CloseApp();

            this.appManagement.HandleAppEvent(app, XAppManagement.kAppEventClose, "");
        }

        #endregion

        #region application active Event

        private void AppClosing(object sender, ClosingEventArgs e)
        {
            XLog.WriteInfo(" AppClosing");
        }

        private void AppDeactivated(object sender, DeactivatedEventArgs e)
        {
            XLog.WriteInfo("AppDeactivated");
        }

        private void AppLaunching(object sender, LaunchingEventArgs e)
        {
            XLog.WriteInfo("AppLaunching");
        }

        private void AppActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            XLog.WriteInfo("AppActivated");
        }

        #endregion

    } 

}
