using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using xFaceLib.runtime;
using xFaceLib.Log;
using BarCodeResult = xFaceLib.extensions.zbar.XBarCodeTask.BarCodeResult;
using xFaceLib.extensions.zbar;

namespace WPCordovaClassLib.Cordova.Commands
{
    public class BarcodeScanner : BaseCommand
    {
        private static String currentCallbackId = null;

        /// <summary>
        /// Used to open a BarcodeScanner application
        /// </summary>
        private XBarCodeTask barCodeTask;

        //FIXME : 采用第三方实现的 com.google.zxing WP7的条码解析库
        // 默认启动二维码解析
        public void start(string options)
        {
            currentCallbackId = JSON.JsonHelper.Deserialize<string[]>(options)[0];

            barCodeTask = new XBarCodeTask();
            barCodeTask.Completed += this.BarCodeTask_Completed;
            barCodeTask.Show();
        }

        private void BarCodeTask_Completed(object sender, BarCodeResult e)
        {
            if (null != e.Error)
            {
                XLog.WriteError("BarCodeTask_Completed occur ERROR " + e.Error.Message);
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, e.Error.Message));
                return;
            }

            switch (e.TaskResult)
            {
                case TaskResult.OK:
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, e.result));
                    break;
                case TaskResult.Cancel:
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Canceled."));
                    break;
                default:
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Did not complete!"));
                    break;
            }
        }
    }
}
