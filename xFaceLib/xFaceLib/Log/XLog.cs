using System;
using System.Collections.Generic;
using xFaceLib.runtime;

namespace xFaceLib.Log
{
    /// <summary>  
    /// Log工具，定义了level，和各级别输出接口 
    /// 默认Log的输出级别是debug，默认listener是XDebugLogListener
    /// </summary>  
    public static class XLog
    {
        /// <summary>
        /// 定义Log的等级
        /// 调用者可以设置level，控制可输出的log
        /// </summary>
        public enum Level
        {
            VERBOSE,
            DEBUG,
            INFO,
            WARN,
            ERROR
        }

        /// <summary>
        /// 默认为debug level
        /// </summary>
        private static Level currentLevel = Level.DEBUG;

        /// <summary>
        /// log全局locker，保证多线程安全
        /// </summary>
        private static readonly object locker = new Object();

        /// <summary>
        /// 所有监听log输出的listener
        /// </summary>
        public static List<XLogListener> Listeners { get; private set; }

        private static XSocketLogListener socketlistener = null;

        static XLog()
        {
            // 这里可以根据config.xml来确定生成哪种listener
            Listeners = new List<XLogListener> {new XDebugLogListener()};
            // 根据xFace.xml来确定初始level
            switch(XSystemConfiguration.GetInstance().LogLevel)
            {
                case "VERBOSE":
                    SetLevel(Level.VERBOSE);
                    break;
                case "DEBUG":
                    SetLevel(Level.DEBUG);
                    break;
                case "INFO":
                    SetLevel(Level.INFO);
                    break;
                case "WARN":
                    SetLevel(Level.WARN);
                    break;
                case "ERROR":
                    SetLevel(Level.ERROR);
                    break;
                default:
                    SetLevel(Level.DEBUG);
                    break;
            }
            
        }

        /// <summary>
        /// 输出verbose级别log
        /// </summary>
        /// <param name="log">log内容</param>
        public static void WriteVerbose(String log)
        {
            // 简单的大lock，这样可以让LogListener的实现也不用考虑thread-safe
            lock (locker)
            {
                if (currentLevel >= Level.VERBOSE)
                {
                    return;
                }

                foreach (var listener in Listeners)
                {
                    listener.LogVerbose(log);
                }
            }
        }

        /// <summary>
        /// 输出debug级别log
        /// </summary>
        /// <param name="log">log内容</param>
        public static void WriteDebug(String log)
        {
            lock (locker)
            {
                if (currentLevel >= Level.DEBUG)
                {
                    return;
                }

                foreach (var listener in Listeners)
                {
                    listener.LogDebug(log);
                }
            }
        }

        /// <summary>
        /// 输出info级别log
        /// </summary>
        /// <param name="log">log内容</param>
        public static void WriteInfo(String log)
        {
            lock (locker)
            {
                if (currentLevel >= Level.INFO)
                {
                    return;
                }

                foreach (var listener in Listeners)
                {
                    listener.LogInfo(log);
                }
            }
        }

        /// <summary>
        /// 输出warn级别log
        /// </summary>
        /// <param name="log">log内容</param>
        public static void WriteWarn(String log)
        {
            lock (locker)
            {
                if (currentLevel >= Level.WARN)
                {
                    return;
                }

                foreach (var listener in Listeners)
                {
                    listener.LogWarn(log);
                }
            }
        }

        /// <summary>
        /// 输出error级别log
        /// </summary>
        /// <param name="log">log内容</param>
        public static void WriteError(String log)
        {
            lock (locker)
            {
                if (currentLevel >= Level.ERROR)
                {
                    return;
                }

                foreach (var listener in Listeners)
                {
                    listener.LogError(log);
                }
            }
        }

        /// <summary>
        /// 设置log级别
        /// </summary>
        /// <param name="level">制定的log级别，对log系统全局有效</param>
        public static void SetLevel(XLog.Level level)
        {
            lock (locker)
            {
                currentLevel = level;
            }
        }

        /// <summary>
        /// 如果有debug.xml这个配置文件，并且设置了ip则调用此构造函数
        /// </summary>
        /// <param name="ip">ip地址</param>
        public static void SetIP(String ip)
        {
            lock (locker)
            {
                if (ip != null)
                {
                    socketlistener = new XSocketLogListener(ip);
                    Listeners.Add(socketlistener);
                }
            }
        }

        public static void Close()
        {
            lock (locker)
            {
                if (socketlistener != null)
                {
                    socketlistener.Close();
                }
                Listeners.Clear();
            }
        }
    }
}
