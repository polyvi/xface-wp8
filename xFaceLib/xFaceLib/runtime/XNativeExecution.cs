using System;
using Microsoft.Phone.Controls;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;

namespace xFaceLib.runtime
{
    public class XNativeExecution : NativeExecution
    {
        public XWebApplication app { internal set; get; }

        public XNativeExecution(WebBrowser browser, XWebApplication application)
            : base(ref browser)
        {
            this.app = application;
        }

        public override void ProcessCommand(CordovaCommandCall commandCallParams)
        {
            BaseCommand bc = CommandFactory.CreateByServiceName(commandCallParams.Service);
            if (bc != null)
            {
                try
                {
                    ((XBaseCommand)bc).app = this.app;
                }
                catch (InvalidCastException)
                {
                }
            }
            base.ProcessCommand(commandCallParams);
        }
    }
}
