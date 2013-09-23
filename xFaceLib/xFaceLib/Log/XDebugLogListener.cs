using System;
using System.Diagnostics;
using xFaceLib.Util;
namespace xFaceLib.Log
{
    /// <summary>  
    /// 提供了XLog的默认listener，方便调试，向console或者
    /// VS output输出log信息 
    /// </summary>  
    public class XDebugLogListener : XLogListener
    {
        public override void LogVerbose(String log)
        {
            Debug.WriteLine("Verbose: " + log);
            WirteToFile("Verbose: " + log);
        }

        public override void LogInfo(String log)
        {
            Debug.WriteLine("Info: " + log);
            WirteToFile("Info: " + log);
        }

        public override void LogWarn(String log)
        {
            UmengSDK.UmengAnalytics.onEvent("DEBUGLOG", log);
            Debug.WriteLine("Warn: " + log);
            WirteToFile("Warn: " + log);
        }

        public override void LogDebug(String log)
        {
            Debug.WriteLine("Debug: " + log);
            WirteToFile("Debug: " + log);
        }

        public override void LogError(String log)
        {
            UmengSDK.UmengAnalytics.onEvent("DEBUGLOG", log);
            Debug.WriteLine("Error: " + log);
            WirteToFile("Error: " + log);
        }
        int position = 0;
        private void WirteToFile(string log)
        {
            log = log + "\n";
            XFile file = new XFile();
            XFile.ErrorCode errCode = null;
            file.Write("log.txt", log, position, out errCode);
            position += log.Length;
        }
    }
}
