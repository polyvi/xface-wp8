using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;
using xFaceLib.runtime;
using xFaceLib.Log;

namespace WPCordovaClassLib.Cordova.Commands
{
    public class App : BaseCommand
    {
        /// <summary>
        /// 调用系统默认程序（浏览器）打开一个url链接(只能是http或https  absolute url )
        /// </summary>
        public void openUrl(string options)
        {
            string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
            string url = args[0];
            try
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri(url, UriKind.RelativeOrAbsolute);
                webBrowserTask.Show();
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
            }
            catch (Exception ex)
            {
                XLog.WriteError("openUrl with ex.Message: " + ex.Message);
                if (ex is ArgumentException || ex is UriFormatException || ex is ArgumentOutOfRangeException)
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                    return;
                }
                throw (ex);
            }
        }

        /// <summary>
        /// 获取渠道信息
        /// </summary>
        public void getChannel(string options)
        {
            XLog.WriteError("NOT SUPPORT!");
            DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
        }

        /// <summary>
        /// 使当前应用返回到前一个页面，函数功能和back按钮相同
        /// </summary>
        public void backHistory(string options)
        {
            XLog.WriteError("NOT SUPPORT!");
            DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
        }

        /// <summary>
        /// 退出xFace引擎程序
        /// </summary>
        public void exitApp(string options)
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
            System.Windows.Application.Current.Terminate();
        }

        /// <summary>
        /// 启动本地应用
        /// </summary>
        public async void startNativeApp(string options)
        {
            string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
            string url = args[0];

            bool c = await Windows.System.Launcher.LaunchUriAsync(new System.Uri(url));
            if (c)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
            }
            else
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
            }
        }
    }
}
