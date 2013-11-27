﻿using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Linq;

namespace xFaceLib.runtime
{
    /// <summary>
    /// 预装包数据项的封装
    /// </summary>
    public class PreInstalPackageItem
    {
        public PreInstalPackageItem(String packagename, String appid)
        {
            PackageName = packagename;
            PackageId = appid;
        }

        public String PackageName { get; set; }
        public String PackageId { get; set; }
    }

    //解析系统配置 xFace的版本信息/支持扩展等
    public class XSystemConfiguration
    {

        /// <summary>
        /// xFace 配置打印log等级
        /// </summary>
        private string logLevel;
        public string LogLevel { get { return logLevel; } }

        /// <summary>
        /// 配置是否显示全屏
        /// </summary>
        private bool isFullscreen;
        public bool IsFullscreen { get { return isFullscreen; } }

        /// <summary>
        /// 配置splash显示
        /// </summary>
        private bool isShowSplash;
        public bool IsShowSplash { get { return isShowSplash; } }

        /// <summary>
        /// 配置splash显示的时间 单位ms
        /// </summary>
        private int splashShowTime;
        public int SplashShowTime { get { return splashShowTime; } }

        /// <summary>
        /// 配置splash是否自动隐藏
        /// </summary>
        private bool autoHideSplashScreen;
        public bool AutoHideSplashScreen { get { return autoHideSplashScreen; } }

        /// <summary>
        /// 工作目录设定策略，1：仅手机内存;2：仅外部存储（FlashROM及SD/TF扩展卡）;3：外部存储优先
        /// </summary>
        private int workDir;
        public int WorkDir { get { return workDir; } }

        /// <summary>
        /// xFace引擎版本号
        /// </summary>
        private string xFaceVersion;
        public string XFaceVersion { get { return xFaceVersion; } }

        /// <summary>
        /// xFace引擎的build号
        /// </summary>
        private string buildNumber;
        public string BuildNumber { get { return buildNumber; } }

        /// <summary>
        /// 配置检测更新的服务器地址
        /// </summary>
        private string updateAddress;
        public string UpdateAddress { get { return updateAddress; } }

        /// <summary>
        /// 是否检查
        /// </summary>
        private bool isCheck;
        public bool IsCheck { get { return isCheck; } }

        /// <summary>
        /// 所有预置安装包的包名
        /// </summary>
        private List<PreInstalPackageItem> prepackedApps;
        public List<PreInstalPackageItem> PrepackedApps { get { return prepackedApps; } }

        /// <summary>
        ///系统工作空间(相对于独立存储的相对路径)
        ///形如：~/xface3/
        /// </summary>
        private string systemWorkspace;
        public string SystemWorkspace { get { return systemWorkspace; } }

        /// <summary>
        /// 应用安装目录(相对于独立存储的相对路径)
        /// 形如：~/xface3/apps/
        /// </summary>
        private string appInstallationDir;
        public string AppInstallationDir { get { return appInstallationDir; } }

        /// <summary>
        /// 应用图标所在目录(相对于独立存储的相对路径)
        /// 形如：~/xface3/app_icons/
        /// </summary>
        private string appIconsDir;
        public string AppIconsDir { get { return appIconsDir; } }   

        private static XSystemConfiguration instance;

        public XSystemConfiguration()
        {
            this.ParsexFaceXml();
            this.systemWorkspace = GetSystemWorkspace();
            this.appIconsDir = GetAppIconsDir();
            this.appInstallationDir = GetAppInstallationDir();
        }

        public static XSystemConfiguration GetInstance()
        {
            if (null == instance)
            {
                instance = new XSystemConfiguration();
            }
            return instance;
        }

        #region parse config.xml
        /// <summary>
        /// 解析config.xml
        /// </summary>
        private void ParsexFaceXml()
        {
            Stream xFaceXml = Application.GetResourceStream(new Uri("config.xml", UriKind.Relative)).Stream;
            XDocument doc = XDocument.Load(xFaceXml);
            XElement widgetElement = doc.Root;

            var values = from results in widgetElement.Descendants()
                         where results.Name.LocalName == "xFace"
                         select results;
            XElement xFace = values.FirstOrDefault();
            ParsexFaceInfo(xFace);

            values = from results in widgetElement.Descendants()
                         where results.Name.LocalName == "pre_install_packages"
                         select results;
            XElement packages = values.FirstOrDefault();
            LoadPreinstallApp(packages);
        }

        /// <summary>
        /// 解析系统配置 获取xFace的信息 如版本号/build号等
        /// </summary>
        private void ParsexFaceInfo(XElement xFaceElement)
        {
            this.logLevel = parsePrefValue(xFaceElement, "LogLevel");
            this.isFullscreen = Convert.ToBoolean(parsePrefValue(xFaceElement, "FullScreen"));
            this.isShowSplash = Convert.ToBoolean(parsePrefValue(xFaceElement, "ShowSplashScreen"));
            this.splashShowTime = int.Parse(parsePrefValue(xFaceElement, "SplashScreenDelayDuration"));
            //AutoHideSplashScreen 没有配置默认为true
            if (null == parsePrefValue(xFaceElement, "AutoHideSplashScreen"))
            {
                this.autoHideSplashScreen = true;
            }
            else
            {
                this.autoHideSplashScreen = Convert.ToBoolean(parsePrefValue(xFaceElement, "AutoHideSplashScreen"));
            }
            this.workDir = int.Parse(parsePrefValue(xFaceElement, "WorkDir"));
            this.xFaceVersion = parsePrefValue(xFaceElement, "EngineVersion");
            this.buildNumber = parsePrefValue(xFaceElement, "EngineBuild");
            this.updateAddress = parsePrefValue(xFaceElement, "UpdateAddress");
            this.isCheck = Convert.ToBoolean(parsePrefValue(xFaceElement, "CheckUpdate"));
        }

        /// <summary>
        /// 解析preference标签中相应name的value值
        /// </summary>
        /// <param name="xFaceElement">xFace元素</param>
        /// <param name="attrName">查找的name值</param>
        /// <returns>对应的value值</returns>
        private String parsePrefValue(XElement xFaceElement,String attrName)
        {
            var itemnodes = from results in xFaceElement.Descendants()
                         where results.Name.LocalName == "preference" && (results.Attribute("name").Value == attrName)
                         select results;

            var itemnode = itemnodes.FirstOrDefault();
            if (itemnode != null)
            {
                return itemnode.Attribute("value").Value;
            }
            return null;
        }

        private void LoadPreinstallApp(XElement pre_install_packages)
        {
            this.prepackedApps = new List<PreInstalPackageItem>();

            var itemnodes = from results in pre_install_packages.Descendants()
                         where results.Name.LocalName == "app_package"
                         select results;

            foreach (XElement itemnode in itemnodes)
            {
                string packageName = itemnode.Value;
                String appId = itemnode.Attribute("id").Value;
                this.prepackedApps.Add(new PreInstalPackageItem(packageName,
                    appId));
            }
        }

        private string GetSystemWorkspace()
        {
            //xFace的工作空间路径形如：C:\Data\Programs\productId\Local\xface3\
            string workSpace = "xface3\\";
            XSystemBootstrap boot = XSystemBootstrapFactory.CreateSystemBootstrap();
            if (boot.GetType().ToString().Equals("xFaceLib.runtime.XPlayerSystemBootstrap"))
            {
                workSpace = "xface_player\\";
            }
            //返回 相对IsolatedStorage 的相对路径
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStorage.DirectoryExists(workSpace))
                {
                    isoStorage.CreateDirectory(workSpace);
                }
            }
            return workSpace;
        }

        private string GetAppIconsDir()
        {
            // 应用图标路径形如：C:\Data\Programs\productId\Local\xface3\app_icons\
            string workSpace = this.systemWorkspace;
            string appIconPath = workSpace + "app_icons\\";
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStorage.DirectoryExists(appIconPath))
                {
                    isoStorage.CreateDirectory(appIconPath);
                }
            }
            return appIconPath;
        }

        private string GetAppInstallationDir()
        {
            // 应用安装路径形如：C:\Data\Programs\productId\Local\xface3\apps\
            string workSpace = this.systemWorkspace;
            string installPath = workSpace + "apps\\";
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStorage.DirectoryExists(installPath))
                {
                    isoStorage.CreateDirectory(installPath);
                }
            }
            return installPath;
        } 

        #endregion
    }
}
