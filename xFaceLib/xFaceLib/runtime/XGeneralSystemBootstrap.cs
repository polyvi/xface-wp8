using System;
using Windows.Storage;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Resources;
using xFaceLib.Util;
using xFaceLib.ams;
using xFaceLib.Log;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Linq;

namespace xFaceLib.runtime
{
    //普通模式的启动引导
    public class XGeneralSystemBootstrap : XSystemBootstrap
    {
        private bool isPreInstallRequired;

        public XGeneralSystemBootstrap()
        {
            this.isPreInstallRequired = false;
        }

        /// <summary>
        /// 启动之前的准备工作
        /// </summary>
        public override void PrepareWorkEnvironment()
        {
            HandleInstalledPackage();

            this.FireFinishToPrepareWorkEnvironment("success");
        }

        /// <summary>
        /// 1. 解析系统的配置文件
        /// 2. 根据配置文件进行预安装
        /// 3. 预安装完后启动默认的app
        /// </summary>
        /// <param name="runtime"></param>
        public override void Boot(XAppManagement appManagement)
        {
            if (this.isPreInstallRequired)
            {
                //预安装,预安装完成启动DefaultApp
                //FIXME：异步处理
                XPreInstallListener listener = new XPreInstallListener(appManagement);
                XPreinstalledAppBatchInstaller preinstalledinstall = new XPreinstalledAppBatchInstaller(appManagement, listener);
                preinstalledinstall.Install();
            }
            else
            {
                //start defaultApp
                string StartParams = XStartParams.GetStartParams();
                appManagement.StartDefaultApp(XStartParams.Parse(StartParams));
            }
        }

        /// <summary>
        /// 处理程序的安装包
        /// </summary>
        private void HandleInstalledPackage()
        {
            if (!XapUpdated())
            {
                return;
            }

            this.isPreInstallRequired = true;

            SaveLastVersion();
        }


        /// <summary>
        /// 当前程序是否首次安装或者覆盖安装过
        /// </summary>
        private bool XapUpdated()
        {
            //版本信息
            string currentVersion = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
            string savedVersion = GetSavedLastVersion();
            return !currentVersion.Equals(savedVersion);
        }

        /// <summary>
        /// 获取上次存储到配置文件中的Xap最后版本号
        /// </summary>
        private string GetSavedLastVersion()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("version"))
            {
                  return (string) IsolatedStorageSettings.ApplicationSettings["version"];
            }
            else
            {
                  return null;
            }
        }

        /// <summary>
        /// 将当前程序的最后版本号存到配置文件中
        /// </summary>
        private void SaveLastVersion()
        {
            string version = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
            userSettings["version"] = version;
            userSettings.Save();
        }
    }
}
