using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Collections.Generic;
using xFaceLib.Log;

namespace xFaceLib.runtime
{
    public class XAppInfo
    {
        /// <summary>
        /// app对应的Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// app的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// app的版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// app 启动页面的相对路径
        /// </summary>
        public string Entry { get; set; }

        /// <summary>
        /// app图标相对路径
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// app图标的背景颜色
        /// </summary>
        public string IconBgColor { get; set; }

        /// <summary>
        /// app对应的类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// app数据是否被加密  
        /// </summary>
        public bool IsEncrypted { get; set; }

        /// <summary>
        /// 应用安装包是否为singleFile模式
        /// </summary>
        public bool IsSingleFileUsed { get; set; }

        /// <summary>
        /// 渠道Id
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 应用的显示方式
        /// </summary>
        public string DisplayMode { get; set; }

        /// <summary>
        /// 应用运行的方式
        /// </summary>
        public string RunningMode { get; set; }

        /// <summary>
        /// 定义此应用基于1.x or 3.x引擎运行
        /// </summary>
        public string EngineType { get; set; }

        /// <summary>
        /// app显示的宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// app显示的高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// app源码是否在Assert下
        /// </summary>
        public bool IsAssets { get; set; }

        /// <summary>
        /// app 对应的引擎版本号
        /// </summary>
        public string EngineVersion { get; set; }

        /// <summary>
        /// native app安装包的store下载地址
        /// </summary>
        public string PrefRemotePkg { get; set; }

        /// <summary>
        /// app.xml中配置的whitelist
        /// </summary>
        public List<string> AccessDomains { get; set; }

        /// <summary>
        /// appInfo对象 初始化
        /// </summary>
        /// <param name="appXmlRelativePath">app.xml的相对独立存储相对路径</param>
        /// <returns>appInfo初始化成功返回 appInfo对象，否则返回null</returns>
        public XAppInfo init(string appXmlRelativePath)
        {
            XLog.WriteInfo("app.xml :: " + appXmlRelativePath);
            if (null == appXmlRelativePath || appXmlRelativePath.Equals(""))
            {
                return null;
            }

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (IsolatedStorageFileStream configFileStream = new IsolatedStorageFileStream(appXmlRelativePath, System.IO.FileMode.Open, storage))
                    {
                        XAppConfigParser parser = XAppConfigParserFactory.CreateAppConfigParser(configFileStream);
                        if (null == parser)
                        {
                            XLog.WriteError("can't parse app.xml :: " + appXmlRelativePath);
                            return null;
                        }

                        return parser.parseConfig();
                    }
                }
                catch (IsolatedStorageException ex)
                {
                    XLog.WriteError("do you have app.xml!" + ex.Message);
                    return null;
                }
            }
        }
    }
}
