using System;
using System.Windows;
using System.Windows.Controls;
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

        public XAppController(Grid layoutRoot)
        {
            this.layoutRoot = layoutRoot;
        }

        public void CreateView(XWebApplication app)
        {
            Uri startPage = app.mode.GetURL(app);
            XAppWebView appView = new XAppWebView(startPage);
            CurrentAppView = appView;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //注册WebApp关联View
                app.SetApp(appView);
                if (app.IsDefaultApp)
                {
                    //DefaultApp才注册load/fail handle处理 splash关闭
                    appView.CDView.Browser.Loaded += AppLoadedHandler;
                    appView.CDView.Browser.NavigationFailed += AppLoadFailedHandler;
                }
                this.layoutRoot.Children.Add(appView.CDView);
                appView.CDView.UpdateLayout();
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
                this.layoutRoot.Children.Remove(appView.CDView);
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

        }

        private void AppLoadedHandler(object sender, RoutedEventArgs e)
        {
            TryStopShowingSplash();
            WebBrowser browser = (WebBrowser)sender;
            browser.Loaded -= AppLoadedHandler;
            browser.NavigationFailed -= AppLoadFailedHandler;
            browser.Visibility = Visibility.Visible;
        }

        private void AppLoadFailedHandler(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            TryStopShowingSplash();
            WebBrowser browser = (WebBrowser)sender;
            browser.Loaded -= AppLoadedHandler;
            browser.NavigationFailed -= AppLoadFailedHandler;
            browser.Visibility = Visibility.Visible;
        }

    }
}
