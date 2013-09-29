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

namespace xFaceLib.runtime
{
    public class XRuntime : XAmsDelegate
    {
        /// <summary>
        /// app应用视图控制器
        /// </summary>
        private XAppController appController;

        private XSystemBootstrap xFaceBoot;

        private XPushNotificationHelper push;

        /// <summary>
        /// 应用管理器
        /// </summary>
        private XAppManagement appManagement;
        public XAppManagement AppManagement
        { get { return appManagement; } }

        private bool isxFaceInited = false;

        public XRuntime(Grid layoutRoot)
        {
            StartupMode mode = PhoneApplicationService.Current.StartupMode;

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
            this.appController.ShowSplashIfNeeded();
        }

        public void HandleBackKeyPress(object sender, CancelEventArgs e)
        {
            this.appController.HandleBackKeyPress(sender, e);
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
            UmengSDK.UmengAnalytics.onLaunching("51c2954456240b164f085721");
            //初始化 push notification 相关
            this.push = new XPushNotificationHelper();
            //异步系统环境准备
             Task.Run(() =>
            {
                if (XSystemConfiguration.GetInstance().IsPlayerMode)
                {
                    this.xFaceBoot = new XPlayerSystemBootstrap();
                }
                else
                {
                    this.xFaceBoot = new XGeneralSystemBootstrap();
                }

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
            //TODO 环境初始化准备失败
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MessageBoxResult result = MessageBox.Show("xFace 初始化失败", "", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    throw new Exception("init failed");//退出
                }
            });
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
            isxFaceInited = true;
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
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("Engine is older than what app requires, may cause issues, please update!");
                        });
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
            UmengSDK.UmengAnalytics.onLaunching("51c2954456240b164f085721");
        }

        private void AppActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            XLog.WriteInfo("AppActivated");
            UmengSDK.UmengAnalytics.onActivated("51c2954456240b164f085721");
        }

        #endregion

    } 

}
