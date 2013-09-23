using System;
using System.IO;

using xFaceLib.Log;
using xFaceLib.runtime;
using xFaceLib.Util;
using xFaceLib.extensions.advancedFileTransfer;

namespace WPCordovaClassLib.Cordova.Commands
{
    public class AdvancedFileTransfer : BaseCommand
    {
        private static String COMMAND_DOWNLOAD = "download";

        private static int FILE_NOT_FOUND_ERR = 1;
        private static int INVALID_URL_ERR = 2;

        private static XFileTransferManager FileTransferManager;

        public AdvancedFileTransfer()
        {
            if (null == FileTransferManager)
            {
                FileTransferManager = new XFileTransferManager();
            }
        }

        public void download(string options)
        {
            string url;
            string filePath;
            string callbackId;

            try
            {
                url = JSON.JsonHelper.Deserialize<string[]>(options)[0];
                filePath = JSON.JsonHelper.Deserialize<string[]>(options)[1];
                callbackId = JSON.JsonHelper.Deserialize<string[]>(options)[2];
            }
            catch (ArgumentException ex)
            {
                XLog.WriteError("download arguments occur Exception JSON_EXCEPTION " + ex.Message);
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
                return;
            }

            String abstarget = XUtils.BuildabsPathOnIsolatedStorage(filePath);
            if (!url.StartsWith("http://"))
            {
                FileTransfer.FileTransferError error = new FileTransfer.FileTransferError(INVALID_URL_ERR, url, filePath, 0);
                PluginResult result = new PluginResult(PluginResult.Status.ERROR, error);
                DispatchCommandResult(result);
                return;
            }
            if (filePath.Contains(":"))
            {
                FileTransfer.FileTransferError error = new FileTransfer.FileTransferError(FILE_NOT_FOUND_ERR, url, filePath, 0);
                PluginResult result = new PluginResult(PluginResult.Status.ERROR, error);
                DispatchCommandResult(result);
                return;
            }

            EventHandler<PluginResult> DispatchPluginResult = delegate(object sender, PluginResult result)
            {
                DispatchCommandResult(result, callbackId);
            };

            FileTransferManager.AddFileTranferTask(url, abstarget,
                    DispatchPluginResult, COMMAND_DOWNLOAD);
        }

        public void cancel(string options)
        {
            String source;
            String target;
            bool isUpload;
            try
            {
                string[] optionStrings = JSON.JsonHelper.Deserialize<string[]>(options);
                source = optionStrings[0];
                target = optionStrings[1];
            }
            catch (ArgumentException ex)
            {
                XLog.WriteError("cancel arguments occur Exception JSON_EXCEPTION " + ex.Message);
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
                return;
            }

            String abstarget = XUtils.BuildabsPathOnIsolatedStorage(target);
            FileTransferManager.Cancel(source, abstarget, COMMAND_DOWNLOAD);
        }

        public void pause(string options)
        {
            String source;
            try
            {
                string[] optionStrings = JSON.JsonHelper.Deserialize<string[]>(options);
                source = optionStrings[0];
            }
            catch (ArgumentException ex)
            {
                XLog.WriteError("pause arguments occur Exception JSON_EXCEPTION " + ex.Message);
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
                return;
            }

            FileTransferManager.Pause(source);
        }

    }
}
