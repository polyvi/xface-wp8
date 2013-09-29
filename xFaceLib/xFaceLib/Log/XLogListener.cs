using System;

namespace xFaceLib.Log
{
    /// <summary>  
    /// ����Log Listener�����ӿ�
    /// Log Listener������ʵ��һ�־���log��������ߣ��������
    /// console, file, remote server�ȣ�ÿһ��listener��Ҫ�ṩ
    /// �Լ���ʵ��
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
