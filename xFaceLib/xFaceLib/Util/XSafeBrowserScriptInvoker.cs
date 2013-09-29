using System;
using Microsoft.Phone.Controls;
using xFaceLib.Log;

namespace xFaceLib.Util
{
    public class XSafeBrowserScriptInvoker
    {
        /// <summary>
        ///  在browser中执行js脚本，若需要脚本返回值，请调用此接口
        /// </summary>
        /// <param name="browser">WebBrowser</param>
        /// <param name="scriptName">待执行的脚本方法名称</param>
        /// <param name="args">传给脚本的参数</param>
        /// <param name="returnedValue">被执行的脚本方法的返回值</param>
        /// <returns>js方法是否调用成功</returns>
        public bool Exec(WebBrowser browser, string scriptName, string[] args, out Object returnedValue)
        {
            try
            {
                returnedValue = browser.InvokeScript(scriptName, args);
                return true;
            }
            catch (Exception ex)
            {
                // 由于InvokeScript可能抛出一些未知异常，必须捕获
                // "80020006"通常为找不到JS方法引起
                // "80020101"通常为JS方法内部错误引起
                XLog.WriteError("Exception while calling js function: " + scriptName);
                XLog.WriteError("Exception Message: " + ex.Message);
                returnedValue = null;
                return false;
            }
        }

        /// <summary>
        ///  在browser中执行js脚本
        /// </summary>
        /// <param name="browser">WebBrowser</param>
        /// <param name="scriptName">待执行的脚本方法名称</param>
        /// <param name="args">传给脚本的参数</param>
        /// <returns>js方法是否调用成功</returns>
        public bool Exec(WebBrowser browser, string scriptName, string[] args)
        {
            try
            {
                browser.InvokeScript(scriptName, args);
                return true;
            }
            catch (Exception ex)
            {
                // 由于InvokeScript确实可能抛出一些未知异常，必须捕获
                XLog.WriteError("Exception while calling js function: " + scriptName);
                XLog.WriteError("Exception Message: " + ex.Message);
                return false;
            }
        }
    }
}
