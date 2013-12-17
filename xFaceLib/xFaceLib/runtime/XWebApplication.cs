using System;
using System.Windows;
using Microsoft.Phone.Controls;
using xFaceLib.Util;
using xFaceLib.Log;
using WPCordovaClassLib.Cordova.JSON;
using WPCordovaClassLib.Cordova;
using System.Collections.Generic;

namespace xFaceLib.runtime
{
    public class XWebApplication : XApplication
    {
        /// <summary>
        /// jsEval器
        /// </summary>
        private XJavaScriptEvaluator jsEvaluator;
        public XJavaScriptEvaluator JsEvaluator
        {
            get { return jsEvaluator; }
        }

        /// <summary>
        /// app的close事件
        /// </summary>
        public event EventHandler<string> AppClose;
        public event EventHandler<string> AppSendMessage;

        /// <summary>
        ///用于存放App的通信数据
        /// </summary>
        private Dictionary<String, Object> datas;

        /// <summary>
        /// APP 存放键值对的共享区
        /// </summary>
        private XDOMStorageHelper DOMStorageHelper;

        public XWebApplication(XAppInfo applicationInfo)
            : base(applicationInfo)
        {
            this.jsEvaluator = new XJavaScriptEvaluator(this);
            this.datas = new Dictionary<string, Object>();
        }

        public void SetApp(XAppWebView AppView)
        {
            this.AppView = AppView;
            this.AppView.IsVaild = true;
            XNativeExecution xFaceExec = new XNativeExecution(this.AppView.Browser, this);
            this.AppView.CDView.nativeExecution = (NativeExecution)xFaceExec;
            DOMStorageHelper = new XDOMStorageHelper();
            DOMStorageHelper.Browser = this.AppView.Browser;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.AppView.Browser.Loaded += XAppWebView_Loaded;
                this.AppView.Browser.LoadCompleted += XAppWebView_LoadCompleted;
                this.AppView.Browser.ScriptNotify += XAppWebView_ScriptNotify;
                this.AppView.Browser.Navigated += XAppWebView_Navigated;
                this.AppView.Browser.Navigating += XAppWebView_Navigating;
                this.AppView.Browser.NavigationFailed += XAppWebView_NavigationFailed;
                this.AppView.Browser.Unloaded += XAppWebView_Unloaded;
            });
        }

        /// <summary>
        /// app 关闭 清理已注册的callback
        /// </summary>
        public override void CloseApp()
        {
            this.AppView.Browser.Loaded -= XAppWebView_Loaded;
            this.AppView.Browser.LoadCompleted -= XAppWebView_LoadCompleted;
            this.AppView.Browser.ScriptNotify -= XAppWebView_ScriptNotify;
            this.AppView.Browser.Navigating -= XAppWebView_Navigating;
            this.AppView.Browser.NavigationFailed -= XAppWebView_NavigationFailed;
            this.AppView.Browser.Unloaded -= XAppWebView_Unloaded;
        }

        /// <summary>
        /// 通知扩展此app已经卸载
        /// </summary>
        public override void OnAppUninstalled()
        {
        }

        #region XApplication EventHandle

        /// <summary>
        /// app 页面切换
        /// </summary>
        private void OnPageStarted()
        {

        }

        /// <summary>
        /// app 页面加载完成
        /// </summary>
        /// <param name="browser"></param>
        private void OnPageLoadCompleted(WebBrowser browser)
        {
            string param = (String) GetData(XConstant.TAG_APP_START_PARAMS);
            if (param != null)
            {
                RemoveData(XConstant.TAG_APP_START_PARAMS);
            }
            //由于app 的localstorage以appId目录存储,需要在onDeviceready fire前初始化privateModule
            //设置privateModule 的初值
            string appId = AppInfo.AppId;
            string workspace = GetWorkSpace();
            workspace = workspace.Replace('\\', '/');
            workspace = "/" + workspace;
            var scriptInvoker = new XSafeBrowserScriptInvoker();
            string appIdResult = "(function() { try { cordova.require('xFace/privateModule').initPrivateData([\"" + appId + "\",\"" + workspace + "\",\"" + param + "\"]);}catch(e){console.log('exception in initPrivateData:' + e);}})();";
            if (!scriptInvoker.Exec(browser, "execScript", new string[] { appIdResult }))
            {
                XLog.WriteError("calling js to initPrivateData. Did you include cordova.js in your html script tag?");
                return;
            }
        }

        #endregion

        private void SendCommand(XCommand command)
        {
            if (null == command || null == command.methodName)
            {
                XLog.WriteError("in exec :: command can not be null!!");
                return;
            }

            //执行App command
            if (TryExecuteXApplicationCmd(command))
            {
                return;
            }
        }

        private bool TryExecuteXApplicationCmd(XCommand cmd)
        {
            if (IsXApplicationCmd(cmd))
            {
                //处理了close事件
                if (cmd.methodName.Equals("closeApplication"))
                {
                    if (null != AppClose)
                    {
                        AppClose(this, AppInfo.AppId);
                        //清空注册的handle
                        AppClose = null;
                        AppSendMessage = null;
                    }
                }
                else
                {
                    //处理appSendMessage 事件
                    string msgId = JsonHelper.Deserialize<string[]>(cmd.arguments)[0];
                    if (null != AppSendMessage)
                    {
                        AppSendMessage(this, msgId);
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsXApplicationCmd(XCommand cmd)
        {
            return cmd.methodName.Equals("closeApplication") || cmd.methodName.Equals("appSendMessage");
        }


        #region webBrowser Event

        private void XAppWebView_Loaded(object sender, RoutedEventArgs e)
        {
            XLog.WriteInfo("XAppWebView_Loaded :: " + e.ToString());
        }

        private void XAppWebView_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            XLog.WriteInfo("XAppWebView_LoadCompleted to :: " + e.Uri.ToString());
            OnPageLoadCompleted((WebBrowser)sender);
        }

        private void XAppWebView_Navigating(object sender, NavigatingEventArgs e)
        {
            XLog.WriteInfo("XAppWebView_Navigating to :: " + e.Uri.ToString());
        }

        private void XAppWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string commandStr = e.Value;
            XLog.WriteInfo("==ScriptNotify");
            XLog.WriteInfo("-------------" + commandStr + "-------------------");

            //忽略 Cordova 已经处理的js command
            if ( (commandStr.IndexOf("XHRLOCAL") == 0) || (commandStr.IndexOf("Orientation") == 0) || (commandStr.IndexOf("ConsoleLog") == 0) )
            {
                return;
            }

            //DOMStorage
            if ((commandStr.IndexOf("DOMStorage") == 0))
            {
                DOMStorageHelper.HandleCommand(commandStr);
                return;
            }

            //FIXME:处理xFace特有的js command
            XCommand command = XCommand.parse(commandStr);
            SendCommand(command);

        }

        private void XAppWebView_Unloaded(object sender, RoutedEventArgs e)
        {
            XLog.WriteInfo("XAppWebView_Unloaded :: " + e.ToString());
        }

        private void XAppWebView_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            XLog.WriteInfo("XAppWebView_NavigationFailed :: " + e.Uri.ToString());
        }

        private void XAppWebView_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            DOMStorageHelper.InjectScript();
            XLog.WriteInfo("XAppWebView_Navigated :: " + e.Uri.ToString());
        }
        #endregion

        /// <summary>
        /// 存放app的数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">数据</param>
        public void SetData(String key, Object value)
        {
            datas.Add(key, value);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="key">键值</param>
        public void RemoveData(String key)
        {
            datas.Remove(key);
        }

        /// <summary>
        /// 获得存放app的数据
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>数据，不存在返回null</returns>
        public Object GetData(String key)
        {
            Object value = null;
            if (datas.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}
