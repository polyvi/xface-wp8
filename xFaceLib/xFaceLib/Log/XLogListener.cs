using System;

namespace xFaceLib.Log
{
    /// <summary>  
    /// 定义Log Listener基本接口
    /// Log Listener可用于实现一种具体log输出接收者，如输出到
    /// console, file, remote server等，每一种listener需要提供
    /// 自己的实现
    /// </summary>  
    public abstract class XLogListener
    {
        public abstract void LogVerbose(String log);
        public abstract void LogInfo(String log);
        public abstract void LogWarn(String log);
        public abstract void LogDebug(String log);
        public abstract void LogError(String log);
    }
}
