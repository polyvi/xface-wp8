using System;
using System.Threading;
using Microsoft.Phone.Controls;
using xFaceLib.Util;
using xFaceLib.Log;

namespace xFaceLib.runtime
{
    public class XJavaScriptEvaluator
    {
        private XWebApplication app;

        private XSafeBrowserScriptInvoker jsInvoker;

        public XJavaScriptEvaluator(XWebApplication app)
        {
            this.app = app;
            this.jsInvoker = new XSafeBrowserScriptInvoker();
        }

        /// <summary>
        /// 执行js语句
        /// </summary>
        /// <param name="jsString">js语句</param>
        public void Eval(string jsString)
        {
            if (String.IsNullOrEmpty(jsString))
            {
                XLog.WriteError("jsString can not be null or empty!!");
                return;
            }

            //替掉行符"\n"为"\\n",否则会引起js语句异常 
            string js = jsString.Replace("\n", "\\n");
            WebBrowser browser = app.AppView.Browser;
            browser.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                jsInvoker.Exec(app.AppView.Browser, "eval", new string[] { jsString });
            });
        }

    }
}
