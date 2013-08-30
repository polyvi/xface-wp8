using System;
using System.Collections.Generic;
using xFaceLib.runtime;

namespace xFaceLib.Log
{
    /// <summary>  
    /// Log���ߣ�������level���͸���������ӿ� 
    /// Ĭ��Log�����������debug��Ĭ��listener��XDebugLogListener
    /// </summary>  
    public static class XLog
    {
        /// <summary>
        /// ����Log�ĵȼ�
        /// �����߿�������level�����ƿ������log
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
        /// Ĭ��Ϊdebug level
        /// </summary>
        private static Level currentLevel = Level.DEBUG;

        /// <summary>
        /// logȫ��locker����֤���̰߳�ȫ
        /// </summary>
        private static readonly object locker = new Object();

        /// <summary>
        /// ���м���log�����listener
        /// </summary>
        public static List<XLogListener> Listeners { get; private set; }

        private static XSocketLogListener socketlistener = null;

        static XLog()
        {
            // ������Ը���config.xml��ȷ����������listener
            Listeners = new List<XLogListener> {new XDebugLogListener()};
            // ����xFace.xml��ȷ����ʼlevel
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
        /// ���verbose����log
        /// </summary>
        /// <param name="log">log����</param>
        public static void WriteVerbose(String log)
        {
            // �򵥵Ĵ�lock������������LogListener��ʵ��Ҳ���ÿ���thread-safe
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
        /// ���debug����log
        /// </summary>
        /// <param name="log">log����</param>
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
        /// ���info����log
        /// </summary>
        /// <param name="log">log����</param>
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
        /// ���warn����log
        /// </summary>
        /// <param name="log">log����</param>
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
        /// ���error����log
        /// </summary>
        /// <param name="log">log����</param>
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
        /// ����log����
        /// </summary>
        /// <param name="level">�ƶ���log���𣬶�logϵͳȫ����Ч</param>
        public static void SetLevel(XLog.Level level)
        {
            lock (locker)
            {
                currentLevel = level;
            }
        }

        /// <summary>
        /// �����debug.xml��������ļ�������������ip����ô˹��캯��
        /// </summary>
        /// <param name="ip">ip��ַ</param>
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
