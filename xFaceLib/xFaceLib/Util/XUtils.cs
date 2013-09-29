using System;
using System.IO;
using System.Windows; 
using ICSharpCode.SharpZipLib.Zip;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Text;
using xFaceLib.runtime;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Storage;
using System.Linq;
using xFaceLib.Log;

namespace xFaceLib.Util
{
    /// <summary>
    /// 提供一些工具方法
    /// </summary>
    public class XUtils
    {
        /// <summary>
        /// 功能：解压zip格式的文件。路径均为绝对路径。
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        /// <returns>解压是否成功</returns>
        public static bool unZipFile(string zipFilePath, string unZipDir)
        {
            try
            {
                if (zipFilePath == string.Empty)
                {
                    XLog.WriteError("Zip packet path is NULL！ ");
                    return false;
                }
                if (!File.Exists(zipFilePath))
                {
                    XLog.WriteError("Cannot find file！ ");
                    return false;
                }
                //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
                if (unZipDir == string.Empty)
                    unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
                if (!unZipDir.EndsWith("\\"))
                    unZipDir += "\\";
                if (!Directory.Exists(unZipDir))
                    Directory.CreateDirectory(unZipDir);

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }
                        if (!directoryName.EndsWith("\\"))
                            directoryName += "\\";
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
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
                        }
                    }//while
                }
                return true;
            }
            catch(ZipException)
            {
                XLog.WriteError("unZipFile " + zipFilePath +" occur error");
                return false;
            }
        }

        /// <summary>
        /// 功能：压缩文件或文件夹，路径均为绝对路径。
        /// </summary>
        /// <param name="dirPath">被压缩的文件或文件夹路径</param>
        /// <param name="zipFilePath">生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名+.zip</param>
        /// <returns>是否压缩成功</returns>
        public static bool ZipFile(string dirPath, string zipFilePath)
        {
            if (dirPath == string.Empty)
            {
                XLog.WriteError("file path is NULL！ ");
                return false;
            }
            bool isFileExist = File.Exists(dirPath);
            bool isDirExist = Directory.Exists(dirPath);
            if (!isFileExist && !isDirExist)
            {
                XLog.WriteError("Cannot find Directory or file！ ");
                return false;
            }
            
            //压缩文件名为空时使用文件夹名＋.zip
            if (zipFilePath == string.Empty)
            {
                if (isDirExist)
                {
                    if (dirPath.EndsWith("\\"))
                    {
                        dirPath = dirPath.Substring(0, dirPath.Length - 1);
                    }
                    zipFilePath = dirPath + ".zip";
                }
                else
                {
                    XLog.WriteError("zipfile path is NULL ");
                    return false;
                }
            }
            else
            {
                string despath = Path.GetDirectoryName(zipFilePath);
                isDirExist = Directory.Exists(despath);
                if (!isDirExist)
                {
                    var info = Directory.CreateDirectory(despath);
                    if (info == null)
                        return false;
                }
            }

            using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
            {
                s.SetLevel(9);
                byte[] buffer = new byte[4096];
                string entryPath = "";
                CompressDir(dirPath, s, entryPath);
            }
            return true;
        }

        /// <summary>
        /// 功能：对多个目录或文件进行zip压缩，路径均为绝对路径。
        /// </summary>
        /// <param name="srcFilePaths">待压缩的文件列表</param>
        /// <param name="zipFileName">要压缩成的zip文件名</param>
        /// <returns>是否压缩成功</returns>
        public static bool ZipFiles(String[] srcFilePaths, String zipFileName)
        {
            if (zipFileName == string.Empty)
            {
                XLog.WriteError("zipFileName is NULL！ ");
                return false;
            }
            if (srcFilePaths == null)
            {
                XLog.WriteError("srcFilePaths is NULL！ ");
                return false;
            }

            using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFileName)))
            {
                s.SetLevel(9);
                foreach(String path in srcFilePaths)
                {
                    String entryPath = "";
                    CompressDir(path, s, entryPath);
                }
            }
            return true;
        }

        /// <summary>
        /// 压缩一个文件或者文件夹
        /// </summary>
        /// <param name="srcFilePath">待压缩的文件或文件夹</param>
        /// <param name="zos">zip输出流</param>
        /// <param name="entryPath">要写入到zip文件中的文件或文件夹的路径</param>
        private static void CompressDir(String srcFilePath, ZipOutputStream zos, String entryPath)
        {
            if (File.Exists(srcFilePath))
            {
                WriteToZip(srcFilePath, zos, entryPath);
            }
            else
            {
                if (Directory.Exists(srcFilePath))
                {
                    string[] filenames = Directory.GetFiles(srcFilePath);
                    if (filenames != null)
                    {
                        foreach (String pathName in filenames)
                        {
                            WriteToZip(pathName, zos, entryPath);
                        }
                    }
                    string[] dirnames = Directory.GetDirectories(srcFilePath);
                    if (dirnames != null)
                    {
                        foreach (String dirname in dirnames)
                        {
                            if (Directory.Exists(dirname))
                            {
                                CompressDir(dirname, zos, entryPath + Path.GetFileName(dirname) + "\\");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将文件写入zip文件中
        /// </summary>
        /// <param name="srcFile">源文件</param>
        /// <param name="zos">zip输出流</param>
        /// <param name="entryPath">要写入到zip文件中的文件或文件夹的路径</param>
        private static void WriteToZip(string srcFile, ZipOutputStream zos, String entryPath)
        {
            ZipEntry anEntry = null;
            string path = entryPath + Path.GetFileName(srcFile);
            if (Directory.Exists(srcFile))
            {
                anEntry = new ZipEntry(path);
                zos.PutNextEntry(anEntry);
                return ;
            }

            anEntry = new ZipEntry(path);
            zos.PutNextEntry(anEntry);
            byte[] buffer = new byte[4096];
            using (FileStream fs = File.OpenRead(srcFile))
            {
                int sourceBytes;
                do
                {
                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                    zos.Write(buffer, 0, sourceBytes);
                } while (sourceBytes > 0);
            }
        }

        /// <summary>
        /// 功能：读取文件。
        /// </summary>
        /// <param name="fileName">文件路径，为LocalFolder下的相对路径</param>
        /// <returns>文件内容字符串</returns>
        public static async Task<string> ReadFile(string fileName)
        {
             byte[] data;
             StorageFolder folder = ApplicationData.Current.LocalFolder;
             StorageFile file = await folder.GetFileAsync(fileName);
             using (Stream s = await file.OpenStreamForReadAsync())
             {
                 data = new byte[s.Length];
                 await s.ReadAsync(data, 0, (int)s.Length);
             }
             return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// 功能：保存文件(异步)。
        /// </summary>
        /// <param name="fileName">文件路径，为LocalFolder下的相对路径</param>
        /// <param name="message">需保存的文件内容</param>
        public static async Task WriteFileAsync(string fileName, string message)
        {
            // Get a reference to the Local Folder
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create the file in the local folder, or if it already exists, just open it
            Windows.Storage.StorageFile storageFile =
                        await localFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

            using (Stream writeStream = await storageFile.OpenStreamForWriteAsync())
            {
                using (StreamWriter writer = new StreamWriter(writeStream))
                {
                    await writer.WriteAsync(message);
                }                
            }
        }

        /// <summary>
        /// 功能：保存文件(同步)。
        /// </summary>
        /// <param name="fileName">文件路径，为LocalFolder下的相对路径</param>
        /// <param name="message">需保存的文件内容</param>
        public static void WriteFile(string fileName, string message)
        {
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoFileStream = isoStorage.CreateFile(fileName))
                {
                    using (StreamWriter writer = new StreamWriter(isoFileStream))
                    {
                        writer.Write(message);
                    }
                }
            }
        }

        /// <summary>
        /// 删除给定路径的目录或文件，如果给定路径是一个目录，则递归删除目录
        /// </summary>
        /// <param name="path">目录/文件路径</param>
        /// <returns>删除是否成功</returns>
        public static bool DeleteFileRecursively(String filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                if (Directory.Exists(filePath))
                {
                    string[] files = Directory.GetFiles(filePath);
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }

                    string[] dirs = Directory.GetDirectories(filePath);
                    foreach (string dir in dirs)
                    {
                        DeleteFileRecursively(dir);
                    }
                    Directory.Delete(filePath);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        //TODO： 以后移到PathResolve
        /// <summary>
        /// 将独立存储上文件的相对路径转为绝对路径
        /// 形如： xFace
        /// 返回： C:\Data\Users\DefApps\AppData\{productId}\Local\xFace\
        /// </summary>
        /// <param name="relativePath">独立存储上文件的相对独立存储的相对路径</param>
        /// <returns>返回绝对路径</returns>
        public static string BuildabsPathOnIsolatedStorage(string relativePath)
        {
            //localFolder fullPath 形如 C:\Data\Users\DefApps\AppData\{productId}\Local\
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            string localFolderPath = localFolder.Path;
            string fullPath = localFolderPath + "\\" + relativePath;
            return fullPath;
        }

        //TODO： 以后移到PathResolve
        /// <summary>
        /// 将安装目录上文件的相对路径转为绝对路径
        /// 形如： www\index.html
        /// 返回： C:\Data\Programs\{productId}\Install\www\index.html
        /// </summary>
        /// <param name="relativePath">安装目录上文件的相对安装目录的相对路径</param>
        /// <returns>返回绝对路径</returns>
        public static string BuildabsPathOnInstallationFolder(string relativePath)
        {
            //InstallationFolder fullPath 形如 C:\Data\Programs\{productId}\Install\
            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string InstallationFolderPath = installationFolder.Path;
            string fullPath = InstallationFolderPath + "\\" + relativePath;
            return fullPath;
        }

        /// <summary>
        /// 从app安装包里面读取出app配置信息
        /// </summary>
        /// <param name="appPackagePath">安装包的路径</param>
        /// <returns>返回绝对路径</returns>
        public static XAppInfo GetAppInfoFromAppPackage(string appPackagePath)
        {
            XAppInfo appInfo = null;
            string APP_CONFIG_FILE_NAME = "app.xml";

            try
            {
                using (ZipInputStream zis = new ZipInputStream(File.OpenRead(appPackagePath)))
                {
                    ZipEntry entry = null;
                    while ((entry = zis.GetNextEntry()) != null)
                    {
                        if (entry.IsDirectory)
                        {
                            continue;
                        }
                        else if (APP_CONFIG_FILE_NAME.Equals(entry.Name))
                        {
                            XAppConfigParser appConfigParser = XAppConfigParserFactory.CreateAppConfigParser(zis);
                            if (null != appConfigParser)
                            {
                                appInfo = appConfigParser.parseConfig();
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException || e is IOException || e is ZipException
                    || e is InvalidOperationException)
                {
                    XLog.WriteError("GetAppInfoFromAppPackage file: " + appPackagePath + " occur Exception: " + e.Message);
                    return null ;
                }
                throw e;
            }
            return appInfo;
        }

        /// <summary>
        /// 拷贝www目录下的js文件到新安装应用所在目录
        /// </summary>
        /// <param name="path">目标目录(相对于独立存储的相对路径)</param>
        public static void copyEmbeddedJsFile(String path)
        {
            //TODO: 拷贝www目录下的cordova.js cordova_plugins.js plugins 到应用安装目录
            string cordovajs = "www\\" + "cordova.js";
            string cordova_pluginsjs = "www\\" + "cordova_plugins.js";
            string pluginsFolder = "www\\" + "plugins";

            string abscordovajs = XUtils.BuildabsPathOnInstallationFolder(cordovajs); ;
            string abscordova_pluginsjs = XUtils.BuildabsPathOnInstallationFolder(cordova_pluginsjs); ;
            string abspluginsFolder = XUtils.BuildabsPathOnInstallationFolder(pluginsFolder); ;

            string destcordovajs = path + "\\" + "cordova.js";
            string destcordova_pluginsjs = path + "\\" + "cordova_plugins.js";
            string absdestcordovajs = XUtils.BuildabsPathOnIsolatedStorage(destcordovajs);
            string absdestcordova_pluginsjs = XUtils.BuildabsPathOnIsolatedStorage(destcordova_pluginsjs);
            string absdestpluginsFolder = XUtils.BuildabsPathOnIsolatedStorage((path + "\\" + "plugins"));

            File.Copy(abscordovajs, absdestcordovajs, true);
            File.Copy(abscordova_pluginsjs, absdestcordova_pluginsjs, true);
            //拷贝plugins目录
            //Directory.Move(abspluginsFolder, absdestpluginsFolder);
        }

        /// <summary>
        /// 拷贝目录或文件
        /// </summary>
        /// <param name="sourceDir">待拷贝的目录或文件(相对于独立存储的相对路径)</param>
        /// <param name="destDir">拷贝的目的目录或文件(相对于独立存储的相对路径)</param>
        public static void Copy(string sourceDir, string destDir)
        {
            if (destDir.Equals(sourceDir))
                return;
            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool isFileExist = isoFile.FileExists(sourceDir);
                bool isDirectoryExist = isoFile.DirectoryExists(sourceDir);
                if (isFileExist)
                {
                    isoFile.CopyFile(sourceDir, destDir, true);
                }
                else if (isDirectoryExist)
                {
                    CopyDirectory(sourceDir, destDir, isoFile);
                }
            }
        }

        private static void CopyDirectory(string sourceDir, string destDir, IsolatedStorageFile isoFile)
        {
            string path;
            if (sourceDir.EndsWith("\\"))
            {
                path = sourceDir;
            }
            else
            {
                path = sourceDir + "\\";
            }

            bool bExists = isoFile.DirectoryExists(destDir);
            if (!bExists)
            {
                isoFile.CreateDirectory(destDir);
            }
            if (!destDir.EndsWith("\\"))
            {
                destDir += "\\";
            }

            string[] files = isoFile.GetFileNames(path + "*");

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    isoFile.CopyFile(path + file, destDir + file, true);
                }
            }
            string[] dirs = isoFile.GetDirectoryNames(path + "*");
            if (dirs.Length > 0)
            {
                foreach (string dir in dirs)
                {
                    CopyDirectory(path + dir, destDir + dir, isoFile);
                }
            }
        }

        /// <summary>
        /// 移动目录或文件
        /// </summary>
        /// <param name="sourcePath">待移动的目录或文件(相对于独立存储的相对路径)</param>
        /// <param name="destPath">移动的目的目录或文件(相对于独立存储的相对路径)，其父目录必须存在</param>
        public static void Move(string sourcePath, string destPath)
        {
            if (destPath.Equals(sourcePath))
                return;
            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool isFileExist = isoFile.FileExists(sourcePath);
                bool isDirectoryExist = isoFile.DirectoryExists(sourcePath);
                if (isFileExist)
                {
                    if (isoFile.FileExists(destPath))
                    {
                        isoFile.DeleteFile(destPath);
                    }
                    isoFile.MoveFile(sourcePath, destPath);
                }
                else if (isDirectoryExist) 
                {
                    if (isoFile.DirectoryExists(destPath))
                    {
                        XUtils.DeleteFileRecursively(XUtils.BuildabsPathOnIsolatedStorage(destPath)); ;
                    }
                    isoFile.MoveDirectory(sourcePath, destPath);
                }
            }
        }

        /// <summary>
        /// 生成应用图标要放置的目标路径
        /// </summary>
        /// <param name="appId">要拷贝的图标所在应用的id</param>
        /// <param name="relativeIconPath">图标在应用内的相对路径</param>
        /// <returns>图标在应用内的相对路径</returns>
        public static String GenerateAppIconPath(String appId, String relativeIconPath)
        {
            if (null == relativeIconPath)
            {
                return null;
            }
            String appDirPath = XSystemConfiguration.GetInstance().AppIconsDir + appId;
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStorage.DirectoryExists(appDirPath))
                {
                    isoStorage.CreateDirectory(appDirPath);
                }
            }
            return appDirPath + "\\" + relativeIconPath;
        }
        
        /// <summary>
        /// 创建临时目录 返回临时目录所创建的目录
        /// </summary>
        /// <param name="parent">临时目录所在的父目录 parent必须要存在</param>
        /// <returns>目录名</returns>
        public static String CreateTempDir(String parent)
        {
            if (!parent.EndsWith("\\"))
            {
                parent += "\\";
            }
            DateTime date1 = DateTime.Now;
            String dirPath = parent + date1.Second.ToString()
                    + XUtils.GenerateRandomId() + "tmp";
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStorage.DirectoryExists(dirPath))
                {
                    isoStorage.CreateDirectory(dirPath);
                }
            }
            return dirPath;
        }


        /// <summary>
        /// 生成一个随机的int类型id
        /// </summary>
        public static int GenerateRandomId()
        {
            Random r = new Random();
            return r.Next();
        }

        /// <summary>
        /// 将路径解析到App的workspace下
        /// </summary>
        /// <returns>返回App workspace 下的路径，无效返回null</returns>
        public static string ResolvePath(string appWorkspace, string path)
        {
            if (null == path)
            {
                return null;
            }
            //路径含有非法字符
            int count = path.IndexOfAny(Path.GetInvalidPathChars());
            if (count > 0)
            {
                return null;
            }
            if (path.Contains(":"))//不支持 含：的完整路径 如C：/xxx/xx
            {
                return null;
            }
            if (path.StartsWith("."))
            {
                return null;
            }
            path = path.Replace("/", "\\");

            if (path.Equals("") || path.Equals("\\"))
            {
                return appWorkspace;
            }

            while (path.StartsWith("\\"))
            {
                //去掉 \ 开头   system.Path 会认为 以 \\开头的是完整路径
                path = path.Substring(1);
            }

            string absLocalPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\";
            string absAppworkspace = BuildabsPathOnIsolatedStorage(appWorkspace);
            string tmp = Path.Combine(absAppworkspace, path);
            tmp = Path.GetFullPath(tmp);// ../ 进入文件的上一级
            tmp = tmp.Substring(absLocalPath.Length);//截取 只返回相对独立存储的路径
            if ( tmp.StartsWith(appWorkspace) )
            {
                return tmp;
            }
            //FIXME: 只处理 在appWorkspace下的路径，其他返回null
            return null;
        }

        /// <summary>
        /// 将时间转换为以毫秒为单位
        /// </summary>
        /// <param name="date">待转换的当前时间</param>
        /// <returns>返回毫秒为单位时间</returns>
        public static double ConvertTimeToMilliSeconds(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalMilliseconds);
        }
    }
}
