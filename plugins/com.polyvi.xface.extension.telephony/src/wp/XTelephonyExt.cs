using System;
using xFaceLib.Util;
using xFaceLib.Log;


namespace WPCordovaClassLib.Cordova.Commands
{
    public class Telephony : BaseCommand
    {

        /// <summary>
        /// 拨打电话
        /// 只支持调出拨打电话询问界面,不支持拨打电话结果状态的获取
        /// </summary>
        public void initiateVoiceCall(string options)
        {
            string phoneNumber;
            try
            {
                string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
                phoneNumber = args[0];
                //check args not empty
                if (phoneNumber.Length > 0) { };
            }
            catch (Exception ex)
            {
                if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException)
                {
                    XLog.WriteError("initiateVoiceCall args error " + ex.Message);
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                    return;
                }
                throw (ex);
            }

            if (!IsTelePhoneNumber(phoneNumber))
            {
                XLog.WriteError("initiateVoiceCall args error: invalid phoneNumber " + phoneNumber);
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                return;
            }

            XSystemTask task = new XSystemTask();
            task.OpenPhoneCall(phoneNumber, null);
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
        }

        private bool IsTelePhoneNumber(string phoneNumber)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[+*#0-9]+$"))//验证拨号键盘能输入的
            {
                return true;
            }
            return false;
        }
    }
}
