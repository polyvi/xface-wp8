using System;
using System.Collections.Generic;
using System.IO;

using xFaceLib.Log;
using xFaceLib.runtime;
using xFaceLib.Util;
using WPCordovaClassLib.Cordova;

namespace xFaceLib.extensions.advancedFileTransfer
{
    public class XFileTransferManager
    {
        private static String COMMAND_DOWNLOAD = "download";

        /// <summary>
        /// 表示为每个source创建一个XIFileTransfer,
        /// 这里的key在下载时表示服务器地址，上传是表示要上传的文件地址
        /// </summary>
        private Dictionary<String, XIFileTransfer> hashMapFileTransfers = new Dictionary<String, XIFileTransfer>();
        public Dictionary<String, XIFileTransfer> HashMapFileTransfers
        {
            get { return hashMapFileTransfers; }
        }

        private XFileTransferRecorder mFileTransferRecorder = new XFileTransferRecorder();

        public XFileTransferManager() {
        }


        /// <summary>
        /// 当文件传输完成后移除XIFileTransfer
        /// </summary>
        /// <param name="source">下载时表示服务器地址，上传时表示要上传的文件地址</param>
        public void RemoveFileTranferTask(String source)
        {
            XIFileTransfer fileTransfer = null;
            HashMapFileTransfers.TryGetValue(source, out fileTransfer);

            if (fileTransfer != null)
            {
                HashMapFileTransfers.Remove(source);
            }
        }

        /// <summary>
        /// 当有文件传输任务发起时，增加一个传输任务
        /// </summary>
        /// <param name="source">下载时表示服务器地址，上传时表示要上传的文件地址</param>
        /// <param name="target">下载时表示存储下载文件的本地地址，上传时表示要上传的服务器地址</param>
        /// <param name="DispatchPluginResult">事件派发</param>
        /// <param name="type">传输的类型(上传或下载两种)</param>
        public void AddFileTranferTask(String source, String target,
                EventHandler<PluginResult> DispatchPluginResult, String type)
        {
            XIFileTransfer fileTransfer = GetFileTransfer(source, target, type);

            fileTransfer.DispatchPluginResult += DispatchPluginResult;
            if(!HashMapFileTransfers.ContainsValue(fileTransfer))
            {
                HashMapFileTransfers.Add(source, fileTransfer);
            }
            fileTransfer.Transfer();
        }

        /// <summary>
        /// 获取XIFileTransfer对象，如果Dictionary<String, XIFileTransfer>中有就直接获取，没有就创建
        /// </summary>
        /// <param name="source">下载时表示服务器地址，上传时表示要上传的文件地址</param>
        /// <param name="target">下载时表示存储下载文件的本地地址，上传时表示要上传的服务器地址</param>
        /// <param name="type">传输的类型(上传或下载两种)</param>
        /// <returns></returns>
        private XIFileTransfer GetFileTransfer(String source, String target, String type)
        {
            XIFileTransfer fileTransfer = null;
            HashMapFileTransfers.TryGetValue(source, out fileTransfer);
            if (fileTransfer == null)
            {
                if (type.Equals(COMMAND_DOWNLOAD))
                {
                    fileTransfer = new XFileDownloader(source, target, mFileTransferRecorder, this);
                }
                HashMapFileTransfers.Add(source, fileTransfer);
            }
            return fileTransfer;
        }


        /// <summary>
        /// 暂停指定的文件传输任务
        /// </summary>
        /// <param name="source">下载时表示服务器地址，上传时表示要上传的文件地址</param>
        public void Pause(String source)
        {
            XIFileTransfer fileTransfer = null;
            HashMapFileTransfers.TryGetValue(source, out fileTransfer);

            if (fileTransfer != null)
            {
                fileTransfer.Pause();
            }
        }


        /// <summary>
        /// 停止所有app中的所有任务
        /// </summary>
        public void StopAll()
        {
            foreach (XIFileTransfer fileTransfer in HashMapFileTransfers.Values)
            {
                fileTransfer.Pause();
            }
        }

        /// <summary>
        /// 取消指定的文件传输任务
        /// </summary>
        /// <param name="source">下载时表示服务器地址，上传时表示要上传的文件地址</param>
        /// <param name="target">下载时表示存储下载文件的本地地址，上传时表示要上传的服务器地址</param>
        /// <param name="type">传输的类型(上传或下载两种)</param>
        public void Cancel(String source, String target, String type)
        {
            Pause(source);
            RemoveFileTranferTask(source);

            if (type.Equals(COMMAND_DOWNLOAD))
            {
                mFileTransferRecorder.DeleteDownloadInfo(source);

                if (File.Exists(target))
                {
                    File.Delete(target);
                }
            }
        }
    }
}
