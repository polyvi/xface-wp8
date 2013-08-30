using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace xFaceLib.Log
{
    public class XSocketLogListener : XLogListener
    {
        private Socket mySocket = null;
        private ManualResetEvent myEvent = null;  

        /// <summary>
        /// debug.cfg�е�tag��ǩ
        /// </summary>
        public static String TAG_SOCKETLOG = "socketlog";
        public static String ATTR_HOST_IP = "hostip";

        /// <summary>
        /// socketlogĬ�ϵķ������˵Ķ˿ں�
        /// </summary>
        public static int HOST_PORT = 6656;

        /// <summary>
        /// socket���ӳ�ʱʱ��
        /// </summary>
        public static int TIME_OUT = 6000;


        public XSocketLogListener(String host)
        {
            ThreadStart connect = () =>
            {
                myEvent = new ManualResetEvent(false);  
                mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
                SocketAsyncEventArgs connArg = new SocketAsyncEventArgs();  
                // Ҫ���ӵ�Զ�̷�����  
                connArg.RemoteEndPoint = new DnsEndPoint(host, HOST_PORT,AddressFamily.InterNetwork);

                connArg.Completed += (sendObj, arg) =>  
                {  
                    if (arg.SocketError == SocketError.Success) //���ӳɹ�  
                    {
                        XLog.WriteInfo("SocketLogServer Connect Success");
                    }  
                    else  
                    {
                        XLog.WriteInfo("SocketLogServer Connect fail");
                        mySocket.Close();
                        mySocket = null;
                    }  
                    // ������̱߳����������  
                    myEvent.Set();
                };

                myEvent.Reset();  
                mySocket.ConnectAsync(connArg);
                myEvent.WaitOne(TIME_OUT);  
            };

            new Thread(connect).Start();
        }

        public override void LogVerbose(String log)
        {
            SendCommand("Verbose: " + log);
        }

        public override void LogDebug(String log)
        {
            SendCommand("Debug: " + log);
        }

        public override void LogInfo(String log)
        {
            SendCommand("Info: " + log);
        }

        public override void LogWarn(String log)
        {
            SendCommand("Warn: " + log);
        }

        public override void LogError(String log)
        {
            SendCommand("Error: " + log);
        }

        /// <summary>
        /// �ر�log
        /// </summary>
        public void Close()
        {
            if (null != mySocket)
            {
                mySocket.Shutdown(SocketShutdown.Both);
                mySocket.Close();
            }
        }

        private void SendCommand(string txt)  
        {  
            if (mySocket != null && mySocket.Connected)
            {  
                SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();
                string msg = txt.Replace("\n", " ") + "\n";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);  
                sendArg.SetBuffer(buffer, 0, buffer.Length);  
                // ������ɺ�Ļص�  
                sendArg.Completed += (objSender, arg) =>  
                    {
                        // �����첽��������  
                        myEvent.Set();  
                    };  
                // �����ź�  
                myEvent.Reset();  
                // �첽����  
                mySocket.SendAsync(sendArg);  
                // �ȴ��������  
                myEvent.WaitOne(TIME_OUT);  
            }  
        }  

    }
}
