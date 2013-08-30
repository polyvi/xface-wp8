using System;
using Microsoft.Phone.Tasks;
using xFaceLib.Log;

namespace xFaceLib.Util
{
    /// <summary>
    /// WP系统相关的Task
    /// </summary>
    public class XSystemTask
    {
        /// <summary>
        /// 打开 WiFi / Bluetooth / Cellular / AirplaneMode 设置界面
        /// </summary>
        /// <param name="type">ConnectionSettingsType</param>
        public void OpenConnectionSetting(ConnectionSettingsType type)
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = type;
            connectionSettingsTask.Show();
        }
        
        /// <summary>
        /// 打开邮件发送界面并设置发送地址/主题/内容/抄送地址(允许参数为null调出邮件界面)
        /// </summary>
        /// <param name="subject">主题(可选)</param>
        /// <param name="body">内容(可选)</param>
        /// <param name="address">发送地址(可选)</param>
        /// address[0] 发送地址
        /// address[1] 抄送地址
        /// address[2] 秘件抄送地址
        public void OpenSendEmail(string subject, string body, string[] address)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = subject;
            emailComposeTask.Body = body;
            if (null != address)
            {
                if (address.Length > 0)//发送地址
                {
                    emailComposeTask.To = address[0];
                }
                if (address.Length > 1)//抄送地址
                {
                    emailComposeTask.Cc = address[1];
                }
                if (address.Length > 2)//秘件抄送地址
                {
                    emailComposeTask.Bcc = address[2];
                }
            }
            emailComposeTask.Show();
        }

        /// <summary>
        /// 打开短信发送界面并设置发送号码/内容(允许参数为null调出短信界面)
        /// </summary>
        /// <param name="to">发送号码(可选)</param>
        /// <param name="body">内容(可选)</param>
        public void OpenSendSms(string to, string body)
        {
            SmsComposeTask smsComposeTask = new SmsComposeTask();

            smsComposeTask.To = to;
            smsComposeTask.Body = body;

            smsComposeTask.Show();

        }

        /// <summary>
        /// 打开通话界面并设置通话号码/显示名字
        /// </summary>
        /// <param name="phoneNumber">通话号码(必须)</param>
        /// <param name="displayName">显示名字(可选)</param>
        public bool OpenPhoneCall(string phoneNumber, string displayName)
        {
            if(string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            PhoneCallTask phoneCallTask = new PhoneCallTask();

            phoneCallTask.PhoneNumber = phoneNumber;
            phoneCallTask.DisplayName = displayName;

            phoneCallTask.Show();
            return true;
        }

        /// <summary>
        /// 打开系统浏览器访问指定的URL(The URI must be absolute and use either HTTP or HTTPS)
        /// </summary>
        /// <param name="url">Uri</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool OpenSystemWebBrowser(Uri url)
        {
            //ArgumentOutOfRangeException
            //The URI must be absolute and use either HTTP or HTTPS
            try
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();

                webBrowserTask.Uri = url;

                webBrowserTask.Show();

                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                XLog.WriteError("OpenSystemWebBrowser " + ex.Message);
                return false;
            }
            catch (NullReferenceException ex)
            {
                XLog.WriteError("OpenSystemWebBrowser " + ex.Message);
                return false;
            }

        }
    }
}
