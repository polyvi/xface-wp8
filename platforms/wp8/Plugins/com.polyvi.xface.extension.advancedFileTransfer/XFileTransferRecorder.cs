using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Linq;

using xFaceLib.Log;
using xFaceLib.runtime;
using xFaceLib.Util;

namespace xFaceLib.extensions.advancedFileTransfer
{
    /// <summary>
    /// 用于操作断点下载/上传配置文件(包括配置文件的初始化，读取配置文件，写配置文件，更新配置文件和删除配置文件
    /// </summary>
    public class XFileTransferRecorder
    {
        private static String WORKSPACE = "fileTransfer";
        private static String TAG_FILETRANSFER_INFO = "\n<filetransfer_info>\n</filetransfer_info>\n";
        private static String FILETRANSFER_CONFIG_FILE_NAME = "filetransfer_info.xml";
        private static String CONFIG_FILE_TAG_DOWNLOAD = "download";
        private static String CONFIG_FILE_TAG_ID = "id";
        private static String CONFIG_FILE_TAG_TOTAL_SIZE = "totalSize";
        private static String CONFIG_FILE_TAG_COMPLETE_SIZE = "completeSize";

        /// <summary>
        /// 配置文件xml内容对应的Document对象
        /// </summary>
        private XDocument Document = null;

        /// <summary>
        /// 配置文件xml的路径
        /// </summary>
        private String ConfigPath;

        public XFileTransferRecorder()
        {
            ConfigPath = WORKSPACE + "\\" + FILETRANSFER_CONFIG_FILE_NAME;

            using(IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.DirectoryExists(WORKSPACE))
                {
                    storage.CreateDirectory(WORKSPACE);
                }
                if(!storage.FileExists(ConfigPath))
                {
                    XUtils.WriteFile(ConfigPath, TAG_FILETRANSFER_INFO);
                }
                try
                {
                    using(IsolatedStorageFileStream configFileStream = new IsolatedStorageFileStream(ConfigPath, System.IO.FileMode.Open, storage))
                    {
                        Document = XDocument.Load(configFileStream);
                    }
                }
                catch (IsolatedStorageException ex)
                {
                    XLog.WriteError("in ParseConfig :: do you have filetransfer_info.xml!" + ex.Message);
                    return ;
                }
                catch (XmlException ex)
                {
                    XLog.WriteError("in ParseConfig :: can't parse filetransfer_info.xml! do you have correct filetransfer_info.xml" + ex.Message);
                    return ;
                }
            }
        }


        /// <summary>
        /// 查看配置文件中是否有该记录
        /// </summary>
        /// <param name="url">要查找的路径</param>
        /// <returns></returns>
        public bool HasDownloadInfo(String url)
        {
            if (null != Document)
            {
                XElement urlElement = GetElementById(url);
                return urlElement != null;
            }
            return false;
        }

        /// <summary>
        /// 保存 下载的具体信息
        /// </summary>
        /// <param name="info">要存储的下载信息，包括文件的地址，文件的大小和已下载的大小</param>
        public void SaveDownloadInfo(XFileDownloadInfo info)
        {
            if(null != Document)
            {
                XElement downloadInfoElement = Document.Root;
                XElement downloadElement = new XElement(CONFIG_FILE_TAG_DOWNLOAD);
                downloadInfoElement.Add(downloadElement);

                downloadElement.SetAttributeValue(CONFIG_FILE_TAG_ID, info.Url);
                downloadElement.SetAttributeValue(CONFIG_FILE_TAG_COMPLETE_SIZE, info.CompleteSize);
                downloadElement.SetAttributeValue(CONFIG_FILE_TAG_TOTAL_SIZE, info.TotalSize);

                XUtils.WriteFile(ConfigPath, Document.ToString());
            }
        }

        /// <summary>
        /// 得到下载具体信息
        /// </summary>
        /// <param name="url">要获取的路径</param>
        /// <returns>下载具体信息</returns>
        public XFileDownloadInfo GetDownloadInfo(String url)
        {
            XFileDownloadInfo info = null;
            if(null != Document)
            {
                XElement downloadElement = GetElementById(url);
                if (downloadElement != null)
                {
                    info = new XFileDownloadInfo(int.Parse(downloadElement
                            .Attribute(CONFIG_FILE_TAG_TOTAL_SIZE).Value), int.Parse(downloadElement
                            .Attribute(CONFIG_FILE_TAG_COMPLETE_SIZE).Value), url);
                }
            }
            return info;
        }

        /// <summary>
        /// 更新配置文件中的下载信息
        /// </summary>
        /// <param name="compeleteSize">已下载的大小</param>
        /// <param name="url">要查找的路径</param>
        public void UpdateDownloadInfo(int compeleteSize, String url)
        {
            if(null != Document)
            {
                XElement downloadElement = GetElementById(url);
                if (downloadElement != null)
                {
                    downloadElement.SetAttributeValue(CONFIG_FILE_TAG_COMPLETE_SIZE,
                            compeleteSize);
                }
                XUtils.WriteFile(ConfigPath, Document.ToString());
            }
        }

        /// <summary>
        /// 下载完成后删除配置文件中的数据
        /// </summary>
        /// <param name="url">要删除的路径</param>
        public void DeleteDownloadInfo(String url)
        {
            if(null != Document)
            {
                XElement downloadInfoElement = (XElement)Document.Root;
                XElement downloadElement = GetElementById(url);
                if (downloadElement != null)
                {
                    downloadElement.Remove();
                }

                XUtils.WriteFile(ConfigPath, Document.ToString());
            }
        }

        /// <summary>
        /// 获取id为指定url的标签
        /// </summary>
        /// <param name="url">查找的标签的url</param>
        /// <returns>对应的标签</returns>
        private XElement GetElementById(String url)
        {
            XElement downloadInfoElement = (XElement)Document.Root;
            foreach (XElement itemnode in downloadInfoElement.Descendants(CONFIG_FILE_TAG_DOWNLOAD))
            {
                string name = itemnode.Attribute(CONFIG_FILE_TAG_ID).Value;
                if (name.Equals(url))
                {
                    return itemnode;
                }
            }
            return null;
        }
    }
}