using System;
using System.Runtime.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Resources;
using System.Text;
using System.Collections.Generic;
using System.Security;
using xFaceLib.runtime;
using xFaceLib.Log;
using xFaceLib.Util;
using WPCordovaClassLib.Cordova.Commands;

namespace xFaceLib.Util
{
    public class XFile
    {
        // Error codes
        public const int NOT_FOUND_ERR = 1;
        public const int SECURITY_ERR = 2;
        public const int ABORT_ERR = 3;
        public const int NOT_READABLE_ERR = 4;
        public const int ENCODING_ERR = 5;
        public const int NO_MODIFICATION_ALLOWED_ERR = 6;
        public const int INVALID_STATE_ERR = 7;
        public const int SYNTAX_ERR = 8;
        public const int INVALID_MODIFICATION_ERR = 9;
        public const int QUOTA_EXCEEDED_ERR = 10;
        public const int TYPE_MISMATCH_ERR = 11;
        public const int PATH_EXISTS_ERR = 12;

        // File system options
        public const int TEMPORARY = 0;
        public const int PERSISTENT = 1;
        public const int RESOURCE = 2;
        public const int APPLICATION = 3;

        /// <summary>
        /// Represents error code for callback
        /// </summary>
        [DataContract]
        public class ErrorCode
        {
            /// <summary>
            /// Error code
            /// </summary>
            [DataMember(IsRequired = true, Name = "code")]
            public int Code { get; set; }

            /// <summary>
            /// Creates ErrorCode object
            /// </summary>
            public ErrorCode(int code)
            {
                this.Code = code;
            }
        }

        /// <summary>
        /// Represents File action options.
        /// </summary>
        [DataContract]
        public class FileOptions
        {
            /// <summary>
            /// File path
            /// </summary>
            private string _fileName;
            [DataMember(Name = "fileName")]
            public string FilePath
            {
                get
                {
                    return this._fileName;
                }

                set
                {
                    int index = value.IndexOfAny(new char[] { '#', '?' });
                    this._fileName = index > -1 ? value.Substring(0, index) : value;
                }
            }

            /// <summary>
            /// Full entryPath
            /// </summary>
            [DataMember(Name = "fullPath")]
            public string FullPath { get; set; }

            /// <summary>
            /// Directory name
            /// </summary>
            [DataMember(Name = "dirName")]
            public string DirectoryName { get; set; }

            /// <summary>
            /// Path to create file/directory
            /// </summary>
            [DataMember(Name = "path")]
            public string Path { get; set; }

            /// <summary>
            /// The encoding to use to encode the file's content. Default is UTF8.
            /// </summary>
            [DataMember(Name = "encoding")]
            public string Encoding { get; set; }

            /// <summary>
            /// Uri to get file
            /// </summary>
            /// 
            private string _uri;
            [DataMember(Name = "uri")]
            public string Uri
            {
                get
                {
                    return this._uri;
                }

                set
                {
                    int index = value.IndexOfAny(new char[] { '#', '?' });
                    this._uri = index > -1 ? value.Substring(0, index) : value;
                }
            }

            /// <summary>
            /// Size to truncate file
            /// </summary>
            [DataMember(Name = "size")]
            public long Size { get; set; }

            /// <summary>
            /// Data to write in file
            /// </summary>
            [DataMember(Name = "data")]
            public string Data { get; set; }

            /// <summary>
            /// Position the writing starts with
            /// </summary>
            [DataMember(Name = "position")]
            public int Position { get; set; }

            /// <summary>
            /// Type of file system requested
            /// </summary>
            [DataMember(Name = "type")]
            public int FileSystemType { get; set; }

            /// <summary>
            /// New file/directory name
            /// </summary>
            [DataMember(Name = "newName")]
            public string NewName { get; set; }

            /// <summary>
            /// Destination directory to copy/move file/directory
            /// </summary>
            [DataMember(Name = "parent")]
            public string Parent { get; set; }

            /// <summary>
            /// Options for getFile/getDirectory methods
            /// </summary>
            [DataMember(Name = "options")]
            public CreatingOptions CreatingOpt { get; set; }

            /// <summary>
            /// Creates options object with default parameters
            /// </summary>
            public FileOptions()
            {
                this.SetDefaultValues(new StreamingContext());
            }

            /// <summary>
            /// Initializes default values for class fields.
            /// Implemented in separate method because default constructor is not invoked during deserialization.
            /// </summary>
            /// <param name="context"></param>
            [OnDeserializing()]
            public void SetDefaultValues(StreamingContext context)
            {
                this.Encoding = "UTF-8";
                this.FilePath = "";
                this.FileSystemType = -1;
            }
        }

        /// <summary>
        /// Stores image info
        /// </summary>
        [DataContract]
        public class FileMetadata
        {
            [DataMember(Name = "name")]
            public string FileName { get; set; }

            [DataMember(Name = "fullPath")]
            public string FullPath { get; set; }

            [DataMember(Name = "type")]
            public string Type { get; set; }

            [DataMember(Name = "lastModifiedDate")]
            public double LastModifiedDate { get; set; }

            [DataMember(Name = "size")]
            public long Size { get; set; }

            public FileMetadata(string filePath)
            {
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (string.IsNullOrEmpty(filePath) || !isoFile.FileExists(filePath))
                    {
                        throw new FileNotFoundException("File doesn't exist");
                    }
                    else
                    {
                        // get file size               
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filePath, FileMode.Open, FileAccess.Read, isoFile))
                        {
                            this.Size = stream.Length;
                        }
                        this.FullPath = filePath;
                        this.FileName = System.IO.Path.GetFileName(filePath);
                        this.LastModifiedDate = XUtils.ConvertTimeToMilliSeconds(isoFile.GetLastWriteTime(filePath).DateTime);
                    }
                    this.Type = MimeTypeMapper.GetMimeType(this.FileName);
                }
            }
        }

        /// <summary>
        /// Represents file or directory modification metadata
        /// </summary>
        [DataContract]
        public class ModificationMetadata
        {
            /// <summary>
            /// Modification time
            /// </summary>
            [DataMember]
            public double modificationTime { get; set; }
        }

        /// <summary>
        /// Represents file or directory entry
        /// </summary>
        [DataContract]
        public class FileEntry
        {

            /// <summary>
            /// File type
            /// </summary>
            [DataMember(Name = "isFile")]
            public bool IsFile { get; set; }

            /// <summary>
            /// Directory type
            /// </summary>
            [DataMember(Name = "isDirectory")]
            public bool IsDirectory { get; set; }

            /// <summary>
            /// File/directory name
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Full path to file/directory
            /// </summary>
            [DataMember(Name = "fullPath")]
            public string FullPath { get; set; }

            public bool IsResource { get; set; }

            public static FileEntry GetEntry(string filePath, bool bIsRes = false)
            {
                FileEntry entry = null;
                try
                {
                    entry = new FileEntry(filePath, bIsRes);
                }
                catch (ArgumentException ex)
                {
                    XLog.WriteError("Exception in GetEntry for filePath :: " + filePath + " " + ex.Message);
                }
                catch (FileNotFoundException ex)
                {
                    XLog.WriteError("Exception in GetEntry for filePath :: " + filePath + " " + ex.Message);
                }
                return entry;
            }

            /// <summary>
            /// Creates object and sets necessary properties
            /// </summary>
            /// <param name="filePath"></param>
            public FileEntry(string filePath, bool bIsRes = false)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("argument is Nullable or empty");
                }

                if (filePath.Contains(" "))
                {
                    XLog.WriteInfo("FilePath with spaces :: " + filePath);
                }

                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsResource = bIsRes;
                    IsFile = isoFile.FileExists(filePath);
                    IsDirectory = isoFile.DirectoryExists(filePath);
                    if (IsFile)
                    {
                        this.Name = Path.GetFileName(filePath);
                    }
                    else if (IsDirectory)
                    {
                        this.Name = this.GetDirectoryName(filePath);
                        if (string.IsNullOrEmpty(Name))
                        {
                            this.Name = "\\";
                        }
                    }
                    else
                    {
                        if (IsResource)
                        {
                            this.Name = Path.GetFileName(filePath);
                        }
                        else
                        {
                            throw new FileNotFoundException("File doesn't exists");
                        }
                    }

                    this.FullPath = filePath;
                }
            }

            /// <summary>
            /// Extracts directory name from path string
            /// Path should refer to a directory, for example \foo\ or /foo.
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            private string GetDirectoryName(string path)
            {
                if (String.IsNullOrEmpty(path))
                {
                    return path;
                }

                string[] split = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 1)
                {
                    return null;
                }
                else
                {
                    return split[split.Length - 1];
                }
            }
        }


        /// <summary>
        /// Represents info about requested file system
        /// </summary>
        [DataContract]
        public class FileSystemInfo
        {
            /// <summary>
            /// file system type
            /// </summary>
            [DataMember(Name = "name", IsRequired = true)]
            public string Name { get; set; }

            /// <summary>
            /// Root directory entry
            /// </summary>
            [DataMember(Name = "root", EmitDefaultValue = false)]
            public FileEntry Root { get; set; }

            /// <summary>
            /// Creates class instance
            /// </summary>
            /// <param name="name"></param>
            /// <param name="rootEntry"> Root directory</param>
            public FileSystemInfo(string name, FileEntry rootEntry = null)
            {
                Name = name;
                Root = rootEntry;
            }
        }

        [DataContract]
        public class CreatingOptions
        {
            /// <summary>
            /// Create file/directory if is doesn't exist
            /// </summary>
            [DataMember(Name = "create")]
            public bool Create { get; set; }

            /// <summary>
            /// Generate an exception if create=true and file/directory already exists
            /// </summary>
            [DataMember(Name = "exclusive")]
            public bool Exclusive { get; set; }

        }

        /// <summary>
        /// 请求一个文件系统来存储应用数据
        /// </summary>
        /// <param name="type">文件系统的类型</param>
        /// <param name="size">指示应用期望的存储大小（bytes）</param>
        /// <param name="errCode">操作返回的错误码值</param>
        /// <returns>返回请求成功的文件系统信息,失败返回null</returns>
        public FileSystemInfo RequestFileSystem(double fileSystemType, double size, out ErrorCode errCode)
        {
            if (size > (10 * 1024 * 1024)) // 10 MB, compier will clean this up!
            {
                errCode = new ErrorCode(QUOTA_EXCEEDED_ERR);
                XLog.WriteError("requestFileSystem with error QUOTA_EXCEEDED_ERR");
                return null;
            }

            try
            {
                if (size != 0)
                {
                    using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        long availableSize = isoFile.AvailableFreeSpace;
                        if (size > availableSize)
                        {
                            errCode = new ErrorCode(QUOTA_EXCEEDED_ERR);
                            XLog.WriteError("requestFileSystem with error QUOTA_EXCEEDED_ERR request size bigger than availableSize");
                            return null;
                        }
                    }
                }
                //FIXME:暂不区分PERSISTENT/TEMPORARY 都同时指向app的Workspace
                if (fileSystemType == PERSISTENT)
                {
                    string persistentEntry = "\\";
                    errCode = null;
                    return new FileSystemInfo("persistent", FileEntry.GetEntry(persistentEntry));
                }
                else if (fileSystemType == TEMPORARY)
                {
                    string tmpFolder = "\\";
                    errCode = null;
                    return new FileSystemInfo("temporary", FileEntry.GetEntry(tmpFolder));
                }
                else
                {
                    errCode = new ErrorCode(NO_MODIFICATION_ALLOWED_ERR);
                    XLog.WriteError("requestFileSystem with error NO_MODIFICATION_ALLOWED_ERR");
                    return null;
                }

            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException)
                {
                    errCode = new ErrorCode(NO_MODIFICATION_ALLOWED_ERR);
                    XLog.WriteError("requestFileSystem with Exception " + ex.Message);
                    return null;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 使用uri获取一个LocalFileSystem的FileEntry或DirectoryEntry
        /// </summary>
        /// <param name="uri">待解析的Uri</param>
        /// <param name="errCode">返回错误码</param>
        /// <returns>返回解析到的FileEntry或DirectoryEntry，失败返回null</returns>
        public FileEntry ResolveLocalFileSystemURI(string uri, out ErrorCode errCode, string appWorkSpace = "")
        {
            try
            {
                uri = Uri.UnescapeDataString(uri);
                Uri fileUri = new Uri(uri);
                string scheme = fileUri.Scheme;
                if ((null == scheme) || (!scheme.Equals("file")))//不是 file:// 协议开头
                {
                    errCode = new ErrorCode(ENCODING_ERR);
                    XLog.WriteError("ResolveLocalFileSystemURI uri: " + uri + " with error code ENCODING_ERR!!");
                    return null;
                }

                string fileProtcal = "file://";
                string path = uri.Substring(fileProtcal.Length);
                if (!string.IsNullOrEmpty(appWorkSpace))
                {
                    path = XUtils.ResolvePath(appWorkSpace, path);
                }
                FileEntry uriEntry = FileEntry.GetEntry(path);
                if (uriEntry != null)
                {
                    errCode = null;
                    return uriEntry;
                }
                else
                {
                    errCode = new ErrorCode(NOT_FOUND_ERR);
                    XLog.WriteError("ResolveLocalFileSystemURI uri: " + uri + " with error code NOT_FOUND_ERR!!");
                    return null;
                }

            }
            catch (ArgumentNullException ex)
            {
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                XLog.WriteError("ResolveLocalFileSystemURI uri can't null, with Exception " + ex.Message);
                return null;
            }
            catch (UriFormatException ex)
            {
                errCode = new ErrorCode(ENCODING_ERR);
                XLog.WriteError("ResolveLocalFileSystemURI uri: " + uri + "  with Exception " + ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 读取文本文件内容，并以base64编码的data url方式返回.
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>base64编码的data url的字符串,失败返回null</returns>
        public string ReadAsDataURL(string filePath, out ErrorCode errCode)
        {
            string base64URL = null;
            if (null == filePath)
            {
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                XLog.WriteError("readAsDataURL file filePath invalid can't be null ,with error code INVALID_MODIFICATION_ERR");
                return base64URL;
            }

            try
            {
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoFile.FileExists(filePath))
                    {
                        XLog.WriteError("readAsDataURL file:" + filePath + " with error code NOT_FOUND_ERR");
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return base64URL;
                    }
                    string mimeType = MimeTypeMapper.GetMimeType(filePath);

                    using (IsolatedStorageFileStream stream = isoFile.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                    {
                        string base64String = GetFileContent(stream);
                        base64URL = "data:" + mimeType + ";base64," + base64String;
                    }
                }

                errCode = null;
                return base64URL;
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException || ex is ArgumentException || ex is IOException)
                {
                    XLog.WriteError("readAsDataURL file:" + filePath + " with error code NOT_READABLE_ERR");
                    XLog.WriteError("readAsDataURL file:" + filePath + " with Exception " + ex.Message);
                    errCode = new ErrorCode(NOT_READABLE_ERR);
                    return base64URL;
                }
                throw ex;
            }

        }

        /// <summary>
        /// 读文本
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="encStr">读取结果的编码 默认UTF-8</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>返回文本字串，失败返回null</returns>
        public string ReadAsText(string filePath, string encStr, out ErrorCode errCode)
        {
            string text = null;
            if (null == filePath)
            {
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                XLog.WriteError("ReadAsText file filePath invalid can't be null ,with error code INVALID_MODIFICATION_ERR");
                return text;
            }
            try
            {
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoFile.FileExists(filePath))
                    {
                        XLog.WriteError("ReadAsText file:" + filePath + " with error code NOT_FOUND_ERR");
                        return ReadResourceAsText(filePath, out errCode);
                    }
                    if (string.IsNullOrEmpty(encStr))
                    {
                        //默认以utf-8编码
                        encStr = "utf-8";
                    }
                    Encoding encoding = Encoding.GetEncoding(encStr);

                    using (TextReader reader = new StreamReader(isoFile.OpenFile(filePath, FileMode.Open, FileAccess.Read), encoding))
                    {
                        text = reader.ReadToEnd();
                    }
                }

                errCode = null;
                return text;
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException || ex is ArgumentException || ex is IOException)
                {
                    XLog.WriteError("ReadAsText file:" + filePath + " with error code NOT_READABLE_ERR");
                    XLog.WriteError("ReadAsText file:" + filePath + " with Exception " + ex.Message);
                    errCode = new ErrorCode(NOT_READABLE_ERR);
                    return text;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 读取资源文本
        /// </summary>
        /// <param name="pathToResource">资源文本路径</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>返回读取的资源文本字串，失败返回null</returns>
        public string ReadResourceAsText(string pathToResource, out ErrorCode errCode)
        {
            string text = null;
            if (null == pathToResource)
            {
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                XLog.WriteError("ReadResourceAsText Resource filePath invalid can't be null ,with error code INVALID_MODIFICATION_ERR");
                return text;
            }

            try
            {
                if (pathToResource.StartsWith("/"))
                {
                    pathToResource = pathToResource.Remove(0, 1);
                }

                var resource = Application.GetResourceStream(new Uri(pathToResource, UriKind.Relative));
                if (resource == null)
                {
                    XLog.WriteError("ReadResourceAsText pathToResource:" + pathToResource + "with error code NOT_FOUND_ERR");
                    errCode = new ErrorCode(NOT_FOUND_ERR);
                    return text;
                }

                StreamReader streamReader = new StreamReader(resource.Stream);
                text = streamReader.ReadToEnd();

                errCode = null;
                return text;
            }
            catch (Exception ex)
            {
                XLog.WriteError("ReadResourceAsText pathToResource:" + pathToResource + " with error code NOT_READABLE_ERR");
                XLog.WriteError("ReadResourceAsText pathToResource:" + pathToResource + " with Exception " + ex.Message);
                errCode = new ErrorCode(NOT_READABLE_ERR);
                return text;
            }

        }

        /// <summary>
        /// 删除指定的文件或空文件夹
        /// </summary>
        /// <param name="filePath">文件(夹)路径</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>成功返回true，相反返回false</returns>
        public bool Remove(string filePath, out ErrorCode errCode)
        {
            if (null == filePath)
            {
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                XLog.WriteError("Remove filePath invalid can't be null, with error INVALID_MODIFICATION_ERR!!");
                return false;
            }

            try
            {
                if (filePath == "/" || filePath == "" || filePath == @"\")
                {
                    errCode = new ErrorCode(NO_MODIFICATION_ALLOWED_ERR);
                    XLog.WriteError("Remove with NO_MODIFICATION_ALLOWED_ERR!(Cannot delete root file system)");
                    return false;
                }
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isoFile.FileExists(filePath))
                    {
                        isoFile.DeleteFile(filePath);
                    }
                    else
                    {
                        if (isoFile.DirectoryExists(filePath))
                        {
                            isoFile.DeleteDirectory(filePath);
                        }
                        else
                        {
                            errCode = new ErrorCode(NOT_FOUND_ERR);
                            XLog.WriteError("Remove filePath: " + filePath + " with error NOT_FOUND_ERR!!");
                            return false;
                        }
                    }
                    errCode = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException)
                {
                    errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                    XLog.WriteError("Remove : " + filePath + "occurs Exception : " + ex.Message);
                    return false;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 递归删除指定的文件夹
        /// </summary>
        /// <param name="filePath">文件夹路径</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>成功返回true，相反返回false</returns>
        public bool RemoveRecursively(string filePath, out ErrorCode errCode)
        {
            if (null == filePath)
            {
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                XLog.WriteError("RemoveRecursively filePath invalid can't be null, with error INVALID_MODIFICATION_ERR!!");
                return false;
            }
            else
            {
                return RemoveDirRecursively(filePath, out errCode);
            }
        }

        /// <summary>
        /// 获取文件FileEntry对象
        /// </summary>
        /// <param name="fOptions">获取文件的可选参数</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>成功返回获取到的文件FileEntry对象，失败返回null</returns>
        public FileEntry GetFile(FileOptions fOptions, out ErrorCode errCode)
        {
            return GetFileOrDirectory(fOptions, false, out errCode);
        }

        /// <summary>
        /// 获取文件夹FileEntry对象
        /// </summary>
        /// <param name="fOptions">获取文件夹的可选参数</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>成功返回获取到的文件夹FileEntry对象，失败返回null</returns>
        public FileEntry GetDirectory(FileOptions fOptions, out ErrorCode errCode)
        {
            return GetFileOrDirectory(fOptions, true, out errCode);
        }

        /// <summary>
        /// 获取指定文件(夹)的Metadata
        /// </summary>
        /// <param name="filePath">指定文件(夹)</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>成功返回获取到的Metadata对象，失败返回null</returns>
        public ModificationMetadata GetMetadata(string filePath, out ErrorCode errCode)
        {
            if (null == filePath)
            {
                XLog.WriteError("GetMetadata filePath can'n be null, with INVALID_MODIFICATION_ERR");
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                return null;
            }

            try
            {
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isoFile.FileExists(filePath))
                    {
                        double modTime = XUtils.ConvertTimeToMilliSeconds(isoFile.GetLastWriteTime(filePath).DateTime);
                        errCode = null;
                        return new ModificationMetadata() { modificationTime = modTime };
                    }
                    else if (isoFile.DirectoryExists(filePath))
                    {
                        double modTime = XUtils.ConvertTimeToMilliSeconds(isoFile.GetLastWriteTime(filePath).DateTime);
                        errCode = null;
                        return new ModificationMetadata() { modificationTime = modTime };
                    }
                    else
                    {
                        XLog.WriteError("GetMetadata filePath: " + filePath + " with NOT_FOUND_ERR");
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException)
                {
                    XLog.WriteError("GetMetadata filePath: " + filePath + " occur Exception " + ex.Message);
                    XLog.WriteError("GetMetadata filePath: " + filePath + " with NOT_READABLE_ERR");
                    errCode = new ErrorCode(NOT_READABLE_ERR);
                    return null;
                }
                throw ex;
            }

        }

        /// <summary>
        /// 获取指定文件的FileMetadata
        /// </summary>
        /// <param name="filePath">指定文件</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>成功返回获取到的FileMetadata对象，失败返回null</returns>
        public FileMetadata GetFileMetadata(string filePath, out ErrorCode errCode)
        {
            if (null == filePath)
            {
                XLog.WriteError("GetFileMetadata filePath can'n be null, with INVALID_MODIFICATION_ERR");
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                return null;
            }

            try
            {
                FileMetadata metaData = new FileMetadata(filePath);
                errCode = null;
                return metaData;
            }
            catch (FileNotFoundException ex)
            {
                XLog.WriteError("GetFileMetadata filePath : " + filePath + " occur Exception " + ex.Message);
                errCode = new ErrorCode(NOT_FOUND_ERR);
                return null;
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException)
                {
                    XLog.WriteError("GetFileMetadata filePath : " + filePath + " occur Exception " + ex.Message);
                    errCode = new ErrorCode(NOT_READABLE_ERR);
                    return null;
                }
                throw ex;
            }

        }

        /// <summary>
        /// 获取指定路径的父目录的FileEntry
        /// </summary>
        /// <param name="filePath">指定文件(夹)</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>成功返回获取到的FileEntry对象，失败返回null</returns>
        public FileEntry GetParent(string filePath, out ErrorCode errCode)
        {
            if (null == filePath)
            {
                XLog.WriteError("GetParent filePath can'n be null, with INVALID_MODIFICATION_ERR");
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                return null;
            }

            try
            {
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    FileEntry entry;

                    if (isoFile.FileExists(filePath) || isoFile.DirectoryExists(filePath))
                    {
                        string path = this.GetParentDirectory(filePath);
                        entry = FileEntry.GetEntry(path);
                        errCode = null;
                        return entry;
                    }
                    else
                    {
                        XLog.WriteError("GetParent filePath " + filePath + " with NOT_FOUND_ERR!!");
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException || 
                    ex is ArgumentException || ex is PathTooLongException)
                {
                    XLog.WriteError("GetParent filePath " + filePath + " occur Exception " + ex.Message);
                    errCode = new ErrorCode(NOT_FOUND_ERR);
                    return null;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 向指定的文件位置处写数据
        /// </summary>
        /// <param name="filePath">指定文件</param>
        /// <param name="data">待写数据</param>
        /// <param name="position">指定位置</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>成功返回写数据的长度，相反返回0</returns>
        public int Write(string filePath, string data, int position, out ErrorCode errCode)
        {
            if (null == filePath)
            {
                XLog.WriteError("Write filePath can'n be null, with INVALID_MODIFICATION_ERR");
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                return 0;
            }
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    XLog.WriteError("Write Expected some data to be send in the write command to " + filePath);
                    errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                    return 0;
                }

                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    // create the file if not exists
                    if (!isoFile.FileExists(filePath))
                    {
                        var file = isoFile.CreateFile(filePath);
                        file.Close();
                    }

                    using (FileStream stream = new IsolatedStorageFileStream(filePath, FileMode.Open, FileAccess.ReadWrite, isoFile))
                    {
                        if (0 <= position && position <= stream.Length)
                        {
                            stream.SetLength(position);
                        }
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Seek(0, SeekOrigin.End);
                            writer.Write(data.ToCharArray());
                        }
                    }
                }

                errCode = null;
                return data.Length;
            }
            catch (DirectoryNotFoundException ex)
            {
                XLog.WriteError("Write file filePath " + filePath + " occur Exception " + ex.Message);
                errCode = new ErrorCode(NOT_FOUND_ERR);
                return 0;
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException ||
                    ex is ArgumentException || ex is IOException || ex is ArgumentOutOfRangeException ||
                    ex is NotSupportedException)
                {
                    XLog.WriteError(string.Format("Write data {0} to file {1} occur Exception {2}", data, filePath, ex.Message));
                    errCode = new ErrorCode(NOT_READABLE_ERR);
                    return 0;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 截取指定大小文件
        /// </summary>
        /// <param name="filePath">指定文件</param>
        /// <param name="size">截取大小</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>返回截取后文件大小</returns>
        public long Truncate(string filePath, int size, out ErrorCode errCode)
        {
            if (null == filePath)
            {
                XLog.WriteError("Truncate filePath can'n be null, with INVALID_MODIFICATION_ERR");
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                return 0;
            }
            try
            {
                long streamLength = 0;
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoFile.FileExists(filePath))
                    {
                        XLog.WriteError(string.Format("Truncate file {0} to with NOT_FOUND_ERR", filePath));
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return 0;
                    }

                    using (FileStream stream = new IsolatedStorageFileStream(filePath, FileMode.Open, FileAccess.ReadWrite, isoFile))
                    {
                        if (0 <= size && size <= stream.Length)
                        {
                            stream.SetLength(size);
                        }
                        streamLength = stream.Length;
                    }
                }

                errCode = null;
                return streamLength;
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException ||
                    ex is ArgumentException || ex is IOException || ex is ArgumentOutOfRangeException ||
                    ex is NotSupportedException)
                {
                    XLog.WriteError(string.Format("Truncate file {0} to size {1} occur Exception {2}", filePath, size, ex.Message));
                    errCode = new ErrorCode(NOT_READABLE_ERR);
                    return 0;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 复制指定文件到指定目录，可以指定复制后的文件名(不指定则使用原文件名)
        /// </summary>
        /// <param name="fullPath">待复制的指定文件</param>
        /// <param name="parent">目标目录</param>
        /// <param name="newFileName">新文件名</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>返回复制后新文件的FileEntry对象，操作失败返回null</returns>
        public FileEntry CopyTo(string fullPath, string parent, string newFileName, out ErrorCode errCode)
        {
            return TransferTo(fullPath, parent, newFileName, false, out errCode);
        }

        /// <summary>
        /// 移动指定文件到指定目录，可以指定移动后的文件名(不指定则使用原文件名)
        /// </summary>
        /// <param name="fullPath">待移动的指定文件</param>
        /// <param name="parent">目标目录</param>
        /// <param name="newFileName">新文件名</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>返回移动后新文件的FileEntry对象，操作失败返回null</returns>
        public FileEntry MoveTo(string fullPath, string parent, string newFileName, out ErrorCode errCode)
        {
            return TransferTo(fullPath, parent, newFileName, true, out errCode);
        }

        /// <summary>
        /// 读取指定文件下的FileEntry
        /// </summary>
        /// <param name="filePath">指定文件</param>
        /// <param name="errCode">返回操作错误码对象</param>
        /// <returns>返回读取的FileEntry对象列表，操作失败返回null</returns>
        public List<FileEntry> ReadEntries(string filePath, out ErrorCode errCode)
        {
            if (null == filePath)
            {
                XLog.WriteError("ReadEntries filePath can'n be null, with INVALID_MODIFICATION_ERR");
                errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                return null;
            }

            try
            {
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isoFile.DirectoryExists(filePath))
                    {
                        string path = TryAppendSlashToDirPath(filePath);
                        List<FileEntry> entries = new List<FileEntry>();
                        string[] files = isoFile.GetFileNames(path + "*");
                        string[] dirs = isoFile.GetDirectoryNames(path + "*");
                        foreach (string file in files)
                        {
                            entries.Add(FileEntry.GetEntry(path + file));
                        }
                        foreach (string dir in dirs)
                        {
                            entries.Add(FileEntry.GetEntry(path + dir + "/"));
                        }

                        errCode = null;
                        return entries;
                    }
                    else
                    {
                        XLog.WriteError("ReadEntries filePath: " + filePath + " with NOT_FOUND_ERR");
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException || ex is ArgumentException)
                {
                    XLog.WriteError("ReadEntries filePath: " + filePath + " occur Exception " + ex.Message);
                    errCode = new ErrorCode(NO_MODIFICATION_ALLOWED_ERR);
                    return null;
                }
                throw ex;
            }
        }

        #region internal private functionality

        /// <summary>
        /// 返回以 base64编码的文件内容字串
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>返回以 base64编码的文件内容字串</returns>
        private string GetFileContent(Stream stream)
        {
            int streamLength = (int)stream.Length;
            byte[] fileData = new byte[streamLength + 1];
            stream.Read(fileData, 0, streamLength);
            stream.Close();
            return Convert.ToBase64String(fileData);
        }

        private bool RemoveDirRecursively(string fullPath, out ErrorCode errCode)
        {
            try
            {
                if (fullPath == "/" || fullPath == "" || fullPath == @"\")
                {
                    errCode = new ErrorCode(NO_MODIFICATION_ALLOWED_ERR);
                    XLog.WriteError("RemoveDirRecursively with NO_MODIFICATION_ALLOWED_ERR!(Cannot delete root file system)");
                    return false;
                }

                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isoFile.DirectoryExists(fullPath))
                    {
                        string tempPath = TryAppendSlashToDirPath(fullPath);
                        string[] files = isoFile.GetFileNames(tempPath + "*");
                        if (files.Length > 0)
                        {
                            foreach (string file in files)
                            {
                                isoFile.DeleteFile(tempPath + file);
                            }
                        }
                        string[] dirs = isoFile.GetDirectoryNames(tempPath + "*");
                        if (dirs.Length > 0)
                        {
                            foreach (string dir in dirs)
                            {
                                if (!RemoveDirRecursively(tempPath + dir, out errCode))
                                {
                                    return false;
                                }
                            }
                        }
                        isoFile.DeleteDirectory(fullPath);
                    }
                    else
                    {
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        XLog.WriteError("RemoveDirRecursively : " + fullPath + "with NOT_FOUND_ERR!!");
                        return false;
                    }
                }
                errCode = null;
                return true;
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException || ex is DirectoryNotFoundException)
                {
                    XLog.WriteError("RemoveDirRecursively : " + fullPath + "occurs Exception : " + ex.Message);
                    errCode = new ErrorCode(NO_MODIFICATION_ALLOWED_ERR);
                    return false;
                }
                throw ex;
            }
        }

        private FileEntry GetFileOrDirectory(FileOptions fOptions, bool getDirectory, out ErrorCode errCode)
        {
            try
            {
                if ((string.IsNullOrEmpty(fOptions.Path)) || (string.IsNullOrEmpty(fOptions.FullPath)))
                {
                    XLog.WriteError("GetFileOrDirectory with path or FullPath error INVALID_MODIFICATION_ERR!!");
                    errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                    return null;
                }
                //非法字符
                if (fOptions.Path.Split(':').Length > 2)
                {
                    XLog.WriteError("GetFileOrDirectory  path with ENCODING_ERR!!");
                    errCode = new ErrorCode(ENCODING_ERR);
                    return null;
                }
                if (fOptions.Path.StartsWith("/"))
                {
                    fOptions.Path = fOptions.Path.Substring(1, fOptions.Path.Length - 1);
                }

                string path;
                try
                {
                    path = Path.Combine(fOptions.FullPath + "/", fOptions.Path);
                }
                catch (ArgumentException)
                {
                    XLog.WriteError("GetFileOrDirectory  path Combine with ENCODING_ERR!!");
                    errCode = new ErrorCode(ENCODING_ERR);
                    return null;
                }
                path = path.Replace("/", "\\");
                XLog.WriteInfo("GetFileOrDirectory  path: " + path);
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    bool isFile = isoFile.FileExists(path);
                    bool isDirectory = isoFile.DirectoryExists(path);
                    bool create = (fOptions.CreatingOpt == null) ? false : fOptions.CreatingOpt.Create;
                    bool exclusive = (fOptions.CreatingOpt == null) ? false : fOptions.CreatingOpt.Exclusive;
                    if (create)
                    {
                        if (exclusive && (isoFile.FileExists(path) || isoFile.DirectoryExists(path)))
                        {
                            errCode = new ErrorCode(PATH_EXISTS_ERR);
                            return null;
                        }

                        if ((getDirectory) && (!isDirectory))
                        {
                            isoFile.CreateDirectory(path);
                        }
                        else
                        {
                            if ((!getDirectory) && (!isFile))
                            {

                                IsolatedStorageFileStream fileStream = isoFile.CreateFile(path);
                                fileStream.Close();
                            }
                        }
                    }
                    else // (not create)
                    {
                        //不存在且不创建 return null
                        if ((!getDirectory) && (isDirectory) || (getDirectory) && (isFile))//获取文件，指定路径为dir 或获取dir指定路径为文件
                        {
                            errCode = new ErrorCode(TYPE_MISMATCH_ERR);
                            return null;
                        }
                        if (((getDirectory) && (!isDirectory)) || ((!getDirectory) && (!isFile)))
                        {
                            errCode = new ErrorCode(NOT_FOUND_ERR);
                            return null;
                        }
                    }

                    //返回获取的文件或文件夹信息
                    FileEntry entry = FileEntry.GetEntry(path);
                    if (entry != null)
                    {
                        errCode = null;
                        return entry;
                    }
                    else
                    {
                        XLog.WriteError("GetFileOrDirectory with error NOT_FOUND_ERR!!");
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException || ex is DirectoryNotFoundException)
                {
                    XLog.WriteError("GetFileOrDirectory occur Exception" + ex.Message);
                    errCode = new ErrorCode(NO_MODIFICATION_ALLOWED_ERR);
                    return null;
                }
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the parent directory name of the specified path,
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Parent directory name</returns>
        private string GetParentDirectory(string path)
        {
            if (String.IsNullOrEmpty(path) || path == "/")
            {
                return "/";
            }

            if (path.EndsWith(@"/") || path.EndsWith(@"\"))
            {
                return this.GetParentDirectory(Path.GetDirectoryName(path));
            }

            string result = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(result))
            {
                result = "/";
            }

            return result;
        }

        private static string TryAppendSlashToDirPath(string dirPath)
        {
            if (dirPath.EndsWith("/"))
            {
                return dirPath;
            }
            else
            {
                return dirPath + "/";
            }
        }

        /// <summary>
        /// 比较两个path是否相同
        /// </summary>
        /// <returns>相同返回true，否则返回false</returns>
        private bool CanonicalCompare(string pathA, string pathB)
        {
            string a = pathA.Replace("/", "\\");
            string b = pathB.Replace("/", "\\");

            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        private void CopyDirectory(string sourceDir, string destDir, IsolatedStorageFile isoFile)
        {
            string path = TryAppendSlashToDirPath(sourceDir);

            bool bExists = isoFile.DirectoryExists(destDir);

            if (!bExists)
            {
                isoFile.CreateDirectory(destDir);
            }

            destDir = TryAppendSlashToDirPath(destDir);

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

        private FileEntry TransferTo(string fullPath, string parent, string newFileName, bool move, out ErrorCode errCode)
        {
            try
            {
                if ((null == newFileName) || (string.IsNullOrEmpty(parent)) || (string.IsNullOrEmpty(fullPath)))
                {
                    XLog.WriteError("Transfer file to path with INVALID_MODIFICATION_ERR");
                    errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                    return null;
                }

                //check newFileName
                char[] invalids = Path.GetInvalidPathChars();
                if (newFileName.IndexOfAny(invalids) > -1 || newFileName.IndexOf(":") > -1)
                {
                    XLog.WriteError("Transfer file to path string ENCODING_ERR");
                    errCode = new ErrorCode(ENCODING_ERR);
                    return null;
                }

                string parentPath = TryAppendSlashToDirPath(parent);
                string currentPath = fullPath;
                string newName;
                //没指定 newFileName 使用oldName
                newName = (string.IsNullOrEmpty(newFileName))
                            ? Path.GetFileName(currentPath)
                            : newFileName;

                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    bool isFileExist = isoFile.FileExists(currentPath);
                    bool isDirectoryExist = isoFile.DirectoryExists(currentPath);
                    bool isParentExist = isoFile.DirectoryExists(parentPath);
                    if (!isParentExist)//目标路径的父目录不存在
                    {
                        XLog.WriteError(string.Format("Transfer file {0} to path {1} with NOT_FOUND_ERR destParent not exist!", fullPath, parent));
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return null;
                    }
                    
                    string newPath;
                    if (isFileExist)//文件移动操作
                    {
                        newPath = Path.Combine(parentPath, newName);

                        // cannot copy file onto itself
                        if (CanonicalCompare(newPath, currentPath))
                        {
                            XLog.WriteError(string.Format("TransferTo file {0} to path {1} can't copy or move onself with INVALID_MODIFICATION_ERR", fullPath, parent));
                            errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                            return null;
                        }
                        else if (isoFile.DirectoryExists(newPath))//如果目标路径为文件夹存在 非法操作
                        {
                            XLog.WriteError("can't move or copy a file to directory!");
                            errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                            return null;
                        }
                        else if (isoFile.FileExists(newPath))//传输文件操作，目标路径存在相应文件 进行删除
                        {   // remove destination file if exists, in other case there will be exception
                            isoFile.DeleteFile(newPath);
                        }

                        if (move)
                        {
                            isoFile.MoveFile(currentPath, newPath);
                        }
                        else
                        {
                            isoFile.CopyFile(currentPath, newPath, true);
                        }
                    }
                    else if (isDirectoryExist)//文件夹移动操作
                    {
                        newName = (string.IsNullOrEmpty(newFileName))
                                    ? currentPath
                                    : newFileName;

                        newPath = Path.Combine(parentPath, newName);
                        // cannot copy or move file onto itself
                        if (CanonicalCompare(newPath, currentPath))
                        {
                            XLog.WriteError(string.Format("TransferTo file {0} to path {1} can't copy or move onself with INVALID_MODIFICATION_ERR", fullPath, parent));
                            errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                            return null;
                        }
                        else if (isoFile.DirectoryExists(newPath))//目标路径存在相应的目录进行删除 可能抛出异常
                        {
                            isoFile.DeleteDirectory(newPath);
                        }

                        if (move)
                        {
                            isoFile.MoveDirectory(currentPath, newPath);
                        }
                        else
                        {
                            CopyDirectory(currentPath, newPath, isoFile);
                        }
                    }
                    else//资源文件不存在
                    {
                        XLog.WriteError(string.Format("Transfer file {0} to path {1} with NOT_FOUND_ERR", fullPath, parent));
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return null;
                    }

                    FileEntry entry = FileEntry.GetEntry(newPath);
                    if (entry != null)
                    {
                        errCode = null;
                        return entry;
                    }
                    else
                    {
                        XLog.WriteError("TransferTo with NOT_FOUND_ERR, don't found in des folder!!");
                        errCode = new ErrorCode(NOT_FOUND_ERR);
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is IsolatedStorageException || ex is ObjectDisposedException || ex is ArgumentException || 
                    ex is DirectoryNotFoundException || ex is InvalidOperationException || ex is FileNotFoundException)
                {
                    XLog.WriteError(string.Format("Transfer file {0} to path {1} with newFileName {2} occur Exception {3}", fullPath, parent, newFileName, ex.Message));
                    errCode = new ErrorCode(INVALID_MODIFICATION_ERR);
                    return null;
                }
                throw ex;
            }
        }

        #endregion
    }
}