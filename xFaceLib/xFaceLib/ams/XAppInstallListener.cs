using System;
using WPCordovaClassLib.Cordova;
namespace xFaceLib.ams
{
    /// <summary>
    /// app的安装监听器类，负责监听app的安装进度及安装结果状态，并封装成消息，发送给mInstallHandler进行处理
    /// </summary>
    public class XAppInstallListener : XInstallListener
    {
        /// <summary>
        /// 安装结果事件派发
        /// </summary>
        public event EventHandler<PluginResult> DispatchPluginResult;


        /// <summary>
        /// 更新安装进度
        /// </summary>
        /// <param name="type">类型标识：安装/卸载</param>
        /// <param name="progressState">进度状态</param>
        public override void OnProgressUpdated(AMS_OPERATION_TYPE type, InstallStatus progressState)
        {
            string res = String.Format("\"progress\":\"{0}\",\"type\":\"{1}\"", ((int)progressState).ToString(), ((int)type).ToString());
            res = "{" + res + "}";
            xFaceLib.Log.XLog.WriteInfo("-------------------AMS OnProgressUpdated result： " + res);

            //TODO: ams install progress
            PluginResult result = new PluginResult(PluginResult.Status.OK, res);
            result.KeepCallback = true;
            DispatchPluginResult(this, result);

        }

        /// <summary>
        /// 安装错误回调
        /// </summary>
        /// <param name="type">类型标识：安装/卸载</param>
        /// <param name="appId">应用id</param>
        /// /// <param name="errorState">错误码</param>
        public override void OnError(AMS_OPERATION_TYPE type, String appId, AMS_ERROR errorState)
        {
            string res = String.Format("\"errorcode\":\"{0}\",\"appid\":\"{1}\",\"type\":\"{2}\"",
                ((int)errorState).ToString(), appId, ((int)type).ToString());
            res = "{" + res + "}";
            xFaceLib.Log.XLog.WriteInfo("-------------------AMS OnError result： " + res);
            PluginResult result = new PluginResult(PluginResult.Status.ERROR, res);
            DispatchPluginResult(this, result);
        }

        /// <summary>
        /// 安装成功回调
        /// </summary>
        /// <param name="type">类型标识：安装/卸载</param>
        /// <param name="appId">应用id</param>
        public override void OnSuccess(AMS_OPERATION_TYPE type, String appId)
        {
            string res = String.Format("\"appid\":\"{0}\",\"type\":\"{1}\"", appId, ((int)type).ToString());
            res = "{" + res + "}";
            xFaceLib.Log.XLog.WriteInfo("--------------AMS OnSuccess result： " + res);
            PluginResult result = new PluginResult(PluginResult.Status.OK, res);
            DispatchPluginResult(this, result);
        }
    }
}
