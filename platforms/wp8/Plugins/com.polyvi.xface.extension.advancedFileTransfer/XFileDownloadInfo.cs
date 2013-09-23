using System;

using xFaceLib.Log;
using xFaceLib.runtime;
using xFaceLib.Util;

namespace xFaceLib.extensions.advancedFileTransfer
{
    /// <summary>
    /// 该类用于记录下载的具体信息（包括下载的地址，下载文件的总大小以及下载完成了的大小,
    /// 这些数据将记录到配置文件中用于断点续传）
    /// </summary>
    public class XFileDownloadInfo
    {
        /// <summary>
        /// 要下载的文件总大小
        /// </summary>
        public int TotalSize { get; set; }

        /// <summary>
        /// 已下载的大小
        /// </summary>
        public int CompleteSize { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string Url { get; private set; }

        public XFileDownloadInfo(int totalSize, int completeSize, String url)
        {
            TotalSize = totalSize;
            CompleteSize = completeSize;
            Url = url;
        }

        public bool IsDownloadCompleted()
        {
            return (TotalSize == CompleteSize) && (TotalSize!= 0);
        }

        public override string ToString()
        {
            return "DownloadInfo [TotalSize=" + TotalSize + ", CompeleteSize="
                    + CompleteSize + ", Url=" + Url + "]";
        }
    }
}
