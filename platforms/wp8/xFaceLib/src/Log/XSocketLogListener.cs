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
        /// debug.cfg中的tag标签
        /// </summary>
        public static String TAG_SOCKETLOG = "socketlog";
        public static String ATTR_HOST_IP = "hostip";

        /// <summary>
        /// socketlog默认的服务器端的端口号
        /// </summary>
        public static int HOST_PORT = 6656;

        /// <summary>
        /// socket连接超时时间
        /// </summary>
        public static int TIME_OUT = 6000;


        public XSocketLogListener(String host)
        {
            ThreadStart connect = () =>
            {
                myEvent = new ManualResetEvent(false);  
                mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
                SocketAsyncEventArgs connArg = new SocketAsyncEventArgs();  
                // 要连接的远程服务器  
                connArg.RemoteEndPoint = new DnsEndPoint(host, HOST_PORT,AddressFamily.InterNetwork);

                connArg.Completed += (sendObj, arg) =>  
                {  
                    if (arg.SocketError == SocketError.Success) //连接成功  
                    {
                        XLog.WriteInfo("SocketLogServer Connect Success");
                    }  
                    else  
                    {
                        XLog.WriteInfo("SocketLogServer Connect fail");
                        mySocket.Close();
                        mySocket = null;
                    }  
                    // 向调用线程报告操作结束  
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
        /// 关闭log
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
                // 发送完成后的回调  
                sendArg.Completed += (objSender, arg) =>  
                    {
                        // 报告异步操作结束  
                        myEvent.Set();  
                    };  
                // 重置信号  
                myEvent.Reset();  
                // 异步发送  
                mySocket.SendAsync(sendArg);  
                // 等待操作完成  
                myEvent.WaitOne(TIME_OUT);  
            }  
        }  

    }
}
