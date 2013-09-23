using System;

namespace xFaceLib.extensions.advancedFileTransfer
{
    public interface XIFileTransferListener
    {

        /// <summary>
        /// 文件传输成功回调
        /// </summary>
        void OnSuccess();

        /// <summary>
        /// 文件输出失败回调
        /// </summary>
        /// <param name="errorCode">失败错误码</param>
        void OnError(int errorCode);

        /// <summary>
        /// 更新传输进度
        /// </summary>
        /// <param name="completeSize">已输出的大小</param>
        /// <param name="totalSize">要传输的总大小</param>
        void OnProgressUpdated(int completeSize, long totalSize);
    }
}
