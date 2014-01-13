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

    }
}
