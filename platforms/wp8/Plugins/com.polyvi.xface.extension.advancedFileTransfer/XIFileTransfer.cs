using System;
using xFaceLib.runtime;
using WPCordovaClassLib.Cordova;
namespace xFaceLib.extensions.advancedFileTransfer
{
    public abstract class XIFileTransfer
    {
        /// <summary>
        /// 安装结果事件派发
        /// </summary>
        public event EventHandler<PluginResult> DispatchPluginResult;

        public void DispatchCommandResult(PluginResult result)
        {
            DispatchPluginResult(this, result);
        }

        /// <summary>
        /// 执行文件传输(上传和下载)
        /// </summary>
        public abstract void Transfer();

        /// <summary>
        /// 暂停文件传输
        /// </summary>
        public abstract void Pause();

    }

}