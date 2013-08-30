using System;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Navigation;
using xFaceLib.Util;
using xFaceLib.Log;
using WPCordovaClassLib;
using WPCordovaClassLib.CordovaLib;

namespace xFaceLib.runtime
{
    public class XAppWebView
    {
        public WebBrowser Browser
        {
            get { return CDView.Browser; }
        }

        /// <summary>
        /// CordovaView
        /// </summary>
        public CordovaView CDView { internal set; get; }

        /// <summary>
        /// 该view是否有效
        /// </summary>
        public bool IsVaild { set; get; }

        /// <summary>
        /// 创建CordovaView,并设置startPageUri
        /// </summary>
        /// <param name="StartPageUri"></param>
        public XAppWebView(Uri StartPageUri)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.IsVaild = false;

                this.CDView = new CordovaView();
                this.CDView.StartPageUri = StartPageUri;
                this.CDView.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                this.CDView.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                this.CDView.Margin = new System.Windows.Thickness(0, 0, 0, 0);

            });
        }

    }
}
