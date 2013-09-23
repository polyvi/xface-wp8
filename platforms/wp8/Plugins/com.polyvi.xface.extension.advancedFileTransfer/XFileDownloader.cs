using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Security;
using System.Reflection;

using xFaceLib.Log;
using xFaceLib.runtime;
using xFaceLib.Util;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;

namespace xFaceLib.extensions.advancedFileTransfer
{
    public class RequestState
    {
        // This class stores the State of the request.
        public int BufferSize { get; set; }
        public byte[] BufferRead { get; set; }
        public HttpWebRequest request { get; set; }
        public HttpWebResponse response { get; set; }
        public Stream streamResponse { get; set; }

        public RequestState()
        {
            BufferRead = null;
            request = null;
            response = null;
            streamResponse = null;
            BufferSize = 0;
        }
    }

    public class XFileDownloader : XIFileTransfer,XIFileTransferListener
    {
        /// <summary>
        /// 定义三种下载的状态：初始化状态，正在下载状态，暂停状态
        /// </summary>
        private const int INIT = 1;
        private const int DOWNLOADING = 2;
        private const int PAUSE = 3;
        private int State = INIT;

        public const int FILENOTFOUND_ERR = 1;
        public const int INVALIDURL_ERR = 2;
        private const int CONNECTION_ERR = 3;
        private const int USERCANCEL_ERR = 4;

        /// <summary>
        /// 定义下载文件的划分倍数和单位文件大小
        /// </summary>
        private const int DIVIDE_SIZE_TWO = 2;
        private const int DIVIDE_SIZE_TEN = 10;
        private const int DIVIDE_SIZE_TWENTY = 20;
        private const int SIZE_KB = 1024;

        /// <summary>
        /// 标示temp文件后缀
        /// </summary>
        private const String TEMP_FILE_SUFFIX = ".temp";

        /// <summary>
        /// 定义下载缓冲区大小
        /// </summary>
        private int BufferSize;

        /// <summary>
        /// 下载器网络标识
        /// </summary>
        private String Url;

        /// <summary>
        /// 存储下载文件的本地路径
        /// </summary>
        private String LocalFilePath;

        /// <summary>
        /// 已下载的具体信息
        /// </summary>
        private XFileDownloadInfo DownloadInfo;

        /// <summary>
        /// 操作配置文件的对象
        /// </summary>
        private XFileTransferRecorder FileTransferRecorder;

        /// <summary>
        /// 下载管理器
        /// </summary>
        private XFileTransferManager FileTransferManager;

        /// <summary>
        /// HttpWebRequest
        /// </summary>
        HttpWebRequest webRequest = null;

        private static readonly object locker = new Object();
        private bool isRun = false;
        private RequestState myRequestState;

        /// <summary>
        /// 安装结果事件派发
        /// </summary>
        //public event EventHandler<PluginResult> DispatchPluginResult;

        public XFileDownloader(String url, String localFilePath,
                XFileTransferRecorder recorder, XFileTransferManager manager)
        {
            Url = url;
            LocalFilePath = localFilePath;
            FileTransferRecorder = recorder;
            FileTransferManager = manager;
            State = INIT;
            myRequestState = new RequestState();
        }

        public override void Transfer()
        {
            if (true == isRun)
            {
                XLog.WriteWarn("Transfer return because isRunning");
                return;
            }
            isRun = true;
            SetState( DOWNLOADING);
            InitDownloadInfo();

            try
            {
                webRequest = (HttpWebRequest)HttpWebRequest.CreateHttp(Url);
                if (DownloadInfo.CompleteSize > 0)
                {
                    webRequest.Headers["Range"] = "bytes=" + DownloadInfo.CompleteSize + "-";//设置Range值
                }
                myRequestState.request = webRequest;
                webRequest.AllowReadStreamBuffering = false;
                webRequest.BeginGetResponse(new AsyncCallback(DownloadCallback), this);
            }
            catch (Exception ex)
            {
                XLog.WriteError("download file with  ConnectionError, occur Exception " + ex.Message);
                if (ex is InvalidOperationException || ex is NotImplementedException || ex is NotSupportedException
                    || ex is ProtocolViolationException || ex is WebException)
                {
                    isRun = false;
                    OnError(CONNECTION_ERR);
                    return;
                }
                throw ex;
            }
        }

        private void DownloadCallback(IAsyncResult asynchronousResult)
        {
            lock (locker)
            {
                if (PAUSE == State)
                {
                    isRun = false;
                    return;
                }
            }

            try
            {
                myRequestState.response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                long totalBytes = myRequestState.response.ContentLength + DownloadInfo.CompleteSize;
                DownloadInfo.TotalSize = (int)totalBytes;
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;
                BufferSize = GetSingleTransferLength(DownloadInfo.TotalSize);
                myRequestState.BufferSize = BufferSize;
                if (myRequestState.BufferRead == null)
                {
                    myRequestState.BufferRead = new byte[BufferSize];
                }
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BufferSize, new AsyncCallback(ReadCallBack), myRequestState);
            }
            catch (Exception ex)
            {
                XLog.WriteError("downloadCallback occur Exception " + ex.Message);
                if (myRequestState.streamResponse != null)
                {
                    myRequestState.streamResponse.Close();
                }

                if (ex is WebException || ex is TargetInvocationException)
                {
                    OnError(CONNECTION_ERR);
                    return;
                }
                else if (ex is ArgumentException || ex is ObjectDisposedException || ex is FileNotFoundException ||
                    ex is IOException || ex is IsolatedStorageException || ex is SecurityException)
                {
                    OnError(FILENOTFOUND_ERR);
                    return;
                }
                isRun = false;
                throw ex;
            }
        }

        private void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {
                lock (locker)
                {
                    if (PAUSE == State)
                    {
                        isRun = false;
                        FileTransferRecorder.UpdateDownloadInfo(DownloadInfo.CompleteSize, DownloadInfo.Url);
                        return;
                    }
                }
                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);

                if (read > 0)
                {
                    using (FileStream fs = System.IO.File.Open(LocalFilePath + TEMP_FILE_SUFFIX, FileMode.Open))
                    {
                        int lStartPos = (int)fs.Length;
                        fs.Seek(lStartPos, System.IO.SeekOrigin.Current); //移动文件流中的当前指针
                        fs.Write(myRequestState.BufferRead, 0, read);
                    }
                    DownloadInfo.CompleteSize += read;
                    OnProgressUpdated(DownloadInfo.CompleteSize, DownloadInfo.TotalSize);
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, myRequestState.BufferSize, new AsyncCallback(ReadCallBack), myRequestState);
                }
                else
                {
                    responseStream.Close();
                    if (DownloadInfo.IsDownloadCompleted())
                    {
                        // 文件下载成功后去掉.temp标示
                        if (System.IO.File.Exists(LocalFilePath))
                        {
                            System.IO.File.Delete(LocalFilePath);
                        }
                        System.IO.File.Move(LocalFilePath + TEMP_FILE_SUFFIX, LocalFilePath);
                        OnSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                XLog.WriteError("downloadCallback occur Exception " + ex.Message);
                myRequestState.streamResponse.Close();
                if (ex is WebException || ex is TargetInvocationException)
                {
                    OnError(CONNECTION_ERR);
                    return;
                }
                else if (ex is ArgumentException || ex is ObjectDisposedException || ex is FileNotFoundException ||
                    ex is IOException || ex is IsolatedStorageException || ex is SecurityException)
                {
                    OnError(FILENOTFOUND_ERR);
                    return;
                }
                isRun = false;
                throw ex;
            }
        }

        public override void Pause()
        {
            lock (locker)
            {
                if (State == DOWNLOADING)
                {
                    SetState(PAUSE);
                }
            }
        }

        public void OnSuccess()
        {
            FileTransferRecorder.DeleteDownloadInfo(Url);
            FileTransferManager.RemoveFileTranferTask(Url);
            SetState(INIT);

            XFile.FileEntry entry = new XFile.FileEntry(LocalFilePath);
            //返回结果 只返回相对appworkspace部分路径

            String absappWorkSpace = XUtils.BuildabsPathOnIsolatedStorage("");
            entry.FullPath = entry.FullPath.Substring(absappWorkSpace.Length);
            entry.FullPath = entry.FullPath.Replace("\\", "/");

            PluginResult result = new PluginResult(PluginResult.Status.OK, entry);
            DispatchCommandResult(result);
            isRun = false;
        }

        public void OnError(int errorCode)
        {
            SetState(INIT);

            FileTransfer.FileTransferError error = new FileTransfer.FileTransferError(errorCode, Url, LocalFilePath, 0);
            PluginResult result = new PluginResult(PluginResult.Status.ERROR, error);
            DispatchCommandResult(result);
            isRun = false;
        }

        public void OnProgressUpdated(int completeSize, long totalSize)
        {
            DownloadInfo.CompleteSize = completeSize;

            string res = String.Format("\"loaded\":\"{0}\",\"total\":\"{1}\"",
                                        completeSize,
                                        totalSize);
            res = "{" + res + "}";
            //TODO: Download progress
        }

        /// <summary>
        /// 初始化下载信息(如果是第一次下载，执行创建本地文件，获取文件的总大小以及在配置文件中添加该条记录，
        /// 如果不是第一次下载，则从配置文件中取出已经下载了的信息，完成断点续传)
        /// </summary>
        private void InitDownloadInfo()
        {
            int totalSize = 0;
            if (!FileTransferRecorder.HasDownloadInfo(Url))
            {
                DownloadInfo = new XFileDownloadInfo(totalSize, 0, Url);
                // 保存mDownloadInfo中的数据到配置文件
                FileTransferRecorder.SaveDownloadInfo(DownloadInfo);

                // 如果第一次下的时候存在temp文件则删除之
                if (System.IO.File.Exists(LocalFilePath + TEMP_FILE_SUFFIX))
                {
                    System.IO.File.Delete(LocalFilePath + TEMP_FILE_SUFFIX);
                }
            }
            else
            {
                // 得到配置文件中已有的url的下载器的具体信息
                DownloadInfo = FileTransferRecorder.GetDownloadInfo(Url);
                totalSize = DownloadInfo.TotalSize;
                if (totalSize == 0)
                {
                    if (System.IO.File.Exists(LocalFilePath + TEMP_FILE_SUFFIX))
                    {
                        System.IO.File.Delete(LocalFilePath + TEMP_FILE_SUFFIX);
                    }
                }
                else
                {
                    DownloadInfo.CompleteSize = GetCompleteSize(LocalFilePath + TEMP_FILE_SUFFIX);
                }
            }

            if (!System.IO.File.Exists(LocalFilePath + TEMP_FILE_SUFFIX))
            {
                FileStream fs = System.IO.File.Create(LocalFilePath + TEMP_FILE_SUFFIX);
                fs.Close();
            }
        }

        /// <summary>
        /// 获取指定文件大小
        /// </summary>
        /// <param name="fileName">文件名字</param>
        /// <returns>文件大小</returns>
        private int GetCompleteSize(String fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                using (FileStream stream = System.IO.File.OpenRead(fileName))
                {
                    return (int)stream.Length;
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取每次要上传文件块的大小 。把下载分成几次更新会使进度条更新更平滑。
        /// 如果文件大小不超过1k，则分成2次更新。<br/>
        /// 如果文件大小在1k-1M之间，则分成10次更新。<br/>
        /// 如果文件大小在1M-10M之间，则分成20次更新。<br/>
        /// 如果文件大小超过10M，则每次下载2M。
        /// </summary>
        private int GetSingleTransferLength(int totalSize)
        {
            // 文件总大小
            int totalLength = totalSize;
            //如果文件小于100字节则直接一次更新进度条
            if (totalLength < SIZE_KB / DIVIDE_SIZE_TEN)
            {
                return SIZE_KB / DIVIDE_SIZE_TEN;
            }
            else if (totalLength < SIZE_KB)
            {
                return totalLength / DIVIDE_SIZE_TWO;
            }
            else if (totalLength < SIZE_KB * SIZE_KB)
            {
                return totalLength / DIVIDE_SIZE_TEN;
            }
            else if (totalLength < DIVIDE_SIZE_TEN * SIZE_KB * SIZE_KB)
            {
                return totalLength / DIVIDE_SIZE_TWENTY;
            }
            else
            {
                return DIVIDE_SIZE_TWO * SIZE_KB * SIZE_KB;
            }
        }

        private void SetState(int state)
        {
            this.State = state;
        }
    }
}
