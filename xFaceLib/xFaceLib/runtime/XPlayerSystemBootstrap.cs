using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Storage;
using System.IO;
using System.IO.IsolatedStorage;
using Windows.Storage;
using System.Threading.Tasks;
using xFaceLib.ams;
using xFaceLib.Util;
using xFaceLib.Log;
using System.Xml;
using System.Xml.Linq;

namespace xFaceLib.runtime
{
    //Player 模式的启动引导
    public class XPlayerSystemBootstrap : XSystemBootstrap
    {
        private static string xFaceInstalledPackageName = "xFaceInstalledPackage.xpa";
        private static string sourceFolderName = "www";

        /// <summary>
        /// 启动之前的准备工作拷贝存储卡上的www文件夹
        /// </summary>
        public override async void PrepareWorkEnvironment()
        {
            InitLogger();

            bool ret = false;
            //todo:Config
            ret = await CopyAssetFilesToIsolatedStoragePath(sourceFolderName, "");
            if (ret && DeployResources())
            {
                //拷贝内置xFace.js到player启动默认应用
                string apppath = XSystemConfiguration.GetInstance().AppInstallationDir + XConstant.DEFAULT_APP_ID_FOR_PLAYER;
                XUtils.copyEmbeddedJsFile(apppath);
                this.FireFinishToPrepareWorkEnvironment("success");
            }
            else
            {
                this.FireFailToPrepareWorkEnvironment("fail");
            }
        }

        /// <summary>
        /// 使用Player模式启动
        /// </summary>
        /// <param name="appManagement"></param>
        public override void Boot(XAppManagement appManagement)
        {
            XApplication app = CreateDefaultApp();
            XApplicationList list = appManagement.GetAppList();
            list.Add(app);
            list.MarkAsDefaultApp(app.AppInfo.AppId);
            string StartParams = XStartParams.GetStartParams();
            appManagement.StartDefaultApp(XStartParams.Parse(StartParams));
        }

        /// <summary>
        /// 将xFaceInstalledPackage.xpa包的应用部署到xFace的工作目录
        /// </summary>
        /// <returns></returns>
        private bool DeployResources()
        {
            string absInstalledPackagePath = XUtils.BuildabsPathOnIsolatedStorage(xFaceInstalledPackageName);
            string dstPath = XSystemConfiguration.GetInstance().AppInstallationDir;
            string absDstPath = XUtils.BuildabsPathOnIsolatedStorage(dstPath + XConstant.DEFAULT_APP_ID_FOR_PLAYER);
            return XUtils.unZipFile(absInstalledPackagePath, absDstPath);
        }

        /// <summary>
        /// 拷贝SD卡文件夹下的文件到独立存储
        /// </summary>
        /// <param name="relativeSDFolder">SD卡上文件夹的相对路径</param>
        /// <param name="relativeIsolatedStorageFolder">独立存储上的相对路径</param>
        /// 注意只能读 系统非保留扩展的文件
        private async Task<bool> CopyAssetFilesToIsolatedStoragePath(string relativeSDFolder, string relativeIsolatedStorageFolder)
        {
            try
            {
                //获取默认SD卡
                ExternalStorageDevice sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();
                if (null != sdCard)
                {
                    //获取应用的www文件夹
                    ExternalStorageFolder sourceFolder = await sdCard.RootFolder.GetFolderAsync(relativeSDFolder);
                    //获取www文件夹下的xFaceInstalledPackage.xpa文件
                    IEnumerable<ExternalStorageFile> sourceFiles = await sourceFolder.GetFilesAsync();

                    foreach (ExternalStorageFile file in sourceFiles)
                    {
                        if (file.Name.Equals(xFaceInstalledPackageName))
                        {
                            Stream sourceStream = await file.OpenForReadAsync();
                            string absPath = XUtils.BuildabsPathOnIsolatedStorage(relativeIsolatedStorageFolder + "\\" + file.Name);
                            using (FileStream streamWriter = File.Create(absPath))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = sourceStream.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                    return true;
                }
                XLog.WriteError("SD card not found !!");
                return false;
            }
            catch (FileNotFoundException)
            {
                XLog.WriteError("File not found " + relativeSDFolder);
                return false;
            }
            catch (IOException)
            {
                XLog.WriteError("error in copy operation " + relativeSDFolder);
                return false;
            }
        }

        /// <summary>
        /// 从debug.xml中读取socket连接服务器的ip地址
        /// </summary>
        /// <returns>返回从debug.xml中读取到的ip地址</returns>
        private async Task<String> ReadIpFromConfig()
        {
            try
            {
                ExternalStorageDevice sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();
                if (null == sdCard)
                    return null;

                ExternalStorageFile file = await sdCard.GetFileAsync("xFacePlayer\\debug.cfg");

                if (null == file)
                    return null;

                String hostIP = null;
                using (Stream s = await file.OpenForReadAsync())
                {
                    XDocument doc = XDocument.Load(s);
                    XElement root = doc.Root;

                    XElement socketlog = root.Element(XSocketLogListener.TAG_SOCKETLOG);
                    if (null != socketlog)
                    {
                        hostIP = socketlog.Attribute(XSocketLogListener.ATTR_HOST_IP).Value;
                    }
                }
                return hostIP;
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is FileNotFoundException || ex is XmlException)
                {
                    XLog.WriteWarn("can't get socket server IP from config with ex: " + ex.Message);
                    return null;
                }
                throw (ex);
            }
        }


        /// <summary>
        /// 根据是否获取到socket服务器ip地方来对XLog进行初始化
        /// </summary>
        private async void InitLogger()
        {
            String hostIP = await ReadIpFromConfig();
            if (null != hostIP)
            {
                XLog.SetIP(hostIP);
            }
        }

        private XApplication CreateDefaultApp()
        {
            //读取 player 包中 defaultapp 配置 app.xml
            //xFace_Player/apps/app/app.xml

            string appRoot = XSystemConfiguration.GetInstance().AppInstallationDir + XConstant.DEFAULT_APP_ID_FOR_PLAYER;
            string appConfigPath = appRoot + "\\" + XConstant.APP_CONFIG_FILE_NAME;
            XAppInfo appInfo = new XAppInfo().init(appConfigPath);

            if (null == appInfo)
            {
                appInfo = new XAppInfo();
                appInfo.IsSingleFileUsed = false;
                appInfo.IsEncrypted = false;
                appInfo.Entry = XConstant.DEFAULT_START_PAGE_NAME;
                appInfo.IsAssets = false;
                appInfo.Type = XConstant.APP_TYPE_XAPP;
                appInfo.Name = "Default";
                appInfo.EngineVersion = XSystemConfiguration.GetInstance().XFaceVersion;
            }
            //player 模式默认使用 DEFAULT_APP_ID_FOR_PLAYER
            appInfo.AppId = XConstant.DEFAULT_APP_ID_FOR_PLAYER;
            XApplication app = XApplicationCreator.Create(appInfo);
            return app;
        }
    }
}
