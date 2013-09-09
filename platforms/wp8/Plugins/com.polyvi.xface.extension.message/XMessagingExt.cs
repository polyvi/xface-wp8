using System;
using xFaceLib.runtime;
using xFaceLib.Util;
using xFaceLib.Log;

namespace WPCordovaClassLib.Cordova.Commands
{
    public class Messaging : BaseCommand
    {
        public const string MESSAGE_TYPE_SMS = "SMS";
        public const string MESSAGE_TYPE_EMAIL = "Email";

        /// <summary>
        /// 发送信息（目前支持发送短信和邮件）
        /// 只支持调用发送信息界面,不支持发送状态结果的获取
        /// </summary>
        public void sendMessage(string options)
        {
            string messageType = "";
            string destAddr = "";
            string messageBody = "";
            string subject = "";

            try
            {
                string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
                if (args.Length > 0) messageType = args[0];//信息的类型（目前支持SMS和Email两种）
                if (args.Length > 1) destAddr = args[1];//要发送的目的地址
                if (args.Length > 2) messageBody = args[2];//要发送的信息内容
                if (args.Length > 3) subject = args[3];//要发送的信息标题(发送邮件时会使用)
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
                {
                    XLog.WriteError("sendMessage args error " + ex.Message);
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                    return;
                }
                throw (ex);
            }

            if (messageType.Equals(MESSAGE_TYPE_SMS))
            {
                XSystemTask task = new XSystemTask();
                task.OpenSendSms(destAddr, messageBody);
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
            }
            else if (messageType.Equals(MESSAGE_TYPE_EMAIL))
            {
                XSystemTask task = new XSystemTask();
                task.OpenSendEmail(subject, messageBody, new string[] { destAddr });
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
            }
            else
            {
                XLog.WriteError("sendMessage args error unkonw messageType");
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
            }
        }
    }
}
