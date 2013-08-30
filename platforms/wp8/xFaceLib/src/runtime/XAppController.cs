using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using System.Windows.Threading;
using xFaceLib.Log;
using xFaceLib.Util;

namespace xFaceLib.runtime
{
    public class XAppController
    {
        /// <summary>
        /// page上用于布局控件的容器
        /// </summary>
        private readonly Grid layoutRoot;

        /// <summary>
        /// 当前显示的appView
        /// </summary>
        public XAppWebView CurrentAppView = null;

        /// <summary>
        /// DOMStorage
        /// </summary>
        private XDOMStorageHelper domStorageHelper;

        /// <summary>
        /// xFace page的backkey事件
        /// </summary>
        private event EventHandler<CancelEventArgs> BackKeyEventHandler;

        public XAppController(Grid layoutRoot)
        {
            this.layoutRoot = layoutRoot;
            this.domStorageHelper = new XDOMStorageHelper();
        }

        public void CreateView(XWebApplication app)
        {
            Uri startPage = app.mode.GetURL(app);
            XAppWebView appView = new XAppWebView(startPage);
            CurrentAppView = appView;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //注册WebApp关联View
                app.SetApp(appView, domStorageHelper);
                this.layoutRoot.Children.Add(appView.CDView);
                appView.CDView.Browser.LoadCompleted += AppLoadCompleteHandler;
                appView.CDView.Browser.NavigationFailed += AppLoadFailedHandler;
            });
        }

        /// <summary>
        /// 关闭指定app的appView
        /// </summary>
        /// <param name="app">待关闭指定的app</param>
        public void CloseView(XWebApplication app)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                XAppWebView appView = app.AppView;
                this.layoutRoot.Children.Remove(appView.Browser);
                appView.IsVaild = false;
                app.AppView = null;
            });
        }

        /// <summary>
        /// 如果设置允许显示splash界面则显示splash
        /// </summary>
        public void ShowSplashIfNeeded()
        {
            //根据配置决定是否显示
            if (!XSystemConfiguration.GetInstance().IsShowSplash)
            {
                return;
            }
            XSplashScreen splash = XSplashScreen.GetInstance();
            splash.ShowxFaceSplash();
        }

        public void HandleBackKeyPress(object sender, CancelEventArgs e)
        {
            XLog.WriteInfo("backkey press ");
            if (layoutRoot.Children.Count > 0)
            {
                string type = layoutRoot.Children[layoutRoot.Children.Count - 1].ToString();

                if (type.Contains("XNotificationBox"))
                {
                    //alert 存在交由alert处理不在传递
                    return;
                }
            }
            if (BackKeyEventHandler != null)
            {
                BackKeyEventHandler(sender, e);
            }
        }

        /// <summary>
        /// 停止显示splash
        /// </summary>
        private void TryStopShowingSplash()
        {
            if (XSystemConfiguration.GetInstance().AutoHideSplashScreen)
            {
                int delayMillsecond = XSystemConfiguration.GetInstance().SplashShowTime;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(delayMillsecond);
                timer.Tick +=  HideSplash;
                timer.Start();
            }
        }

        private void HideSplash(object sender, EventArgs arg)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= HideSplash;

            XSplashScreen splash = XSplashScreen.GetInstance();
            splash.Hide();
            //SystemTray
            Microsoft.Phone.Shell.SystemTray.IsVisible = true;
        }

        private void AppLoadCompleteHandler(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            TryStopShowingSplash();
            WebBrowser browser = (WebBrowser)sender;
            browser.LoadCompleted -= AppLoadCompleteHandler;
            browser.NavigationFailed -= AppLoadFailedHandler;
            browser.Visibility = Visibility.Visible;
        }

        private void AppLoadFailedHandler(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            TryStopShowingSplash();
            WebBrowser browser = (WebBrowser)sender;
            browser.NavigationFailed -= AppLoadFailedHandler;
            browser.Visibility = Visibility.Visible;
        }

    }
}
