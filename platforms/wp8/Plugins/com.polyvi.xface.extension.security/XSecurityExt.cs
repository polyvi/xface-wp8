using System;
using System.Text;
using System.Runtime.Serialization;
using xFaceLib.Log;
using System.IO;
using System.IO.IsolatedStorage;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace WPCordovaClassLib.Cordova.Commands
{
    public enum CryptAlgorithm : int
    {
        DES = 1,
        TRIPLE_DES = 2,
        RSA = 3
    };

    public enum SecurityError : int
    {
        FILE_NOT_FOUND_ERR = 1,
        PATH_ERR = 2,
        OPERATION_ERR = 3
    };

    public enum EncodeType : int
    {
        XDataUTF8Encoding = 0, //UTF8编码
        XDataBase64Encoding = 1, //base64编码
        XDataHexEncoding = 2  //16进制编码
    };

    /// <summary>
    /// Represents Security action options.
    /// </summary>
    [DataContract]
    public class SecurityOptions
    {
        /// <summary>
        /// CryptAlgorithm
        /// </summary>
        [DataMember(Name = "CryptAlgorithm")]
        public CryptAlgorithm CryptAlgorithm { get; set; }

        /// <summary>
        /// EncodeDataType
        /// </summary>
        [DataMember(Name = "EncodeDataType")]
        public EncodeType EncodeDataType { get; set; }

        /// <summary>
        /// EncodeKeyType
        /// </summary>
        [DataMember(Name = "EncodeKeyType")]
        public EncodeType EncodeKeyType { get; set; }

        /// <summary>
        /// Creates options object with default parameters
        /// </summary>
        public SecurityOptions()
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
            this.CryptAlgorithm = CryptAlgorithm.DES;//DES
            this.EncodeDataType = EncodeType.XDataBase64Encoding;//BASE64
            this.EncodeKeyType = EncodeType.XDataUTF8Encoding;//UTF8
        }
    }

    public class Security : BaseCommand
    {

        private const int SECURITY_KEY_MIN_LENGTH = 8;

        /// <summary>
        /// 使用指定算法加密,通过回调返回加密后的数据.
        /// </summary>
        /// <param name="options"></param>
        /// command.arguments:
        /// - 1 sKey 密钥
        /// - 2 sourceData 需要加密的数据
        /// - 3 jsOptions  需要加密的数据可选参数
        ///     - 1 CryptAlgorithm        加密算法，默认des
        ///     - 2 EncodeDataType        数据编码格式，默认base64
        ///     - 3 kKeyForEncodeKeyType  密钥编码格式，默认16进制
        public void encrypt(string options)
        {
            string sKey = string.Empty;
            string sourceDataStr = string.Empty;

            string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
            sKey = args[0];
            sourceDataStr = args[1];
            SecurityOptions option = new SecurityOptions();
            if (null != args[2])
            {
                option = JSON.JsonHelper.Deserialize<SecurityOptions>(args[2]);
            }

            //check
            if (string.IsNullOrEmpty(sKey) || sKey.Length < 8 || string.IsNullOrEmpty(sourceDataStr))
            {
                XLog.WriteError("Input data invalid!");
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                return;
            }

            try
            {
                var keyData = StringToData(sKey, option.EncodeKeyType);
                keyData = MakeKey(keyData, option.CryptAlgorithm);
                var sourceData = Encoding.UTF8.GetBytes(sourceDataStr);

                byte[] result = Encrypt(keyData, sourceData, option.CryptAlgorithm);

                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, DataToString(result, option.EncodeDataType)));

            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is ArgumentNullException || ex is ArgumentException || ex is ArgumentOutOfRangeException || ex is NotSupportedException)
                {
                    XLog.WriteError("encrypt data error with Exception : " + ex.Message);
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                }
            }
        }

        /// <summary>
        /// 使用指定算法解密,通过回调返回解密后的数据.
        /// </summary>
        /// <param name="options"></param>
        /// command.arguments:
        /// - 1 sKey 密钥
        /// - 2 sourceData 需要解密的数据
        /// - 3 jsOptions  需要解密的数据可选参数
        ///     - 1 CryptAlgorithm        加密算法，默认des
        ///     - 2 EncodeDataType        数据编码格式，默认base64
        ///     - 3 kKeyForEncodeKeyType  密钥编码格式，默认16进制
        public void decrypt(string options)
        {
            string sKey = string.Empty;
            string sourceDataStr = string.Empty;

            string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
            sKey = args[0];
            sourceDataStr = args[1];
            SecurityOptions option = new SecurityOptions();
            if (null != args[2])
            {
                option = JSON.JsonHelper.Deserialize<SecurityOptions>(args[2]);
            }

            //check
            if (string.IsNullOrEmpty(sKey) || sKey.Length < 8 || string.IsNullOrEmpty(sourceDataStr))
            {
                XLog.WriteError("Input data invalid!");
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                return;
            }

            try
            {
                var keyData = StringToData(sKey, option.EncodeKeyType);
                keyData = MakeKey(keyData, option.CryptAlgorithm);
                var sourceData = StringToData(sourceDataStr, option.EncodeDataType);

                byte[] result = Decrypt(keyData, sourceData, option.CryptAlgorithm);
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, Encoding.UTF8.GetString(result, 0, result.Length)));
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is ArgumentNullException || ex is ArgumentException || ex is ArgumentOutOfRangeException || ex is NotSupportedException)
                {
                    XLog.WriteError("decrypt data error with Exception : " + ex.Message);
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                }
            }
        }

        /// <summary>
        /// 使用指定算法加密文件,通过回调返回加密后的数据.
        /// </summary>
        /// <param name="options"></param>
        /// command.arguments:
        /// - 1 sKey 密钥
        /// - 2 sourceFile 需要加密的文件
        /// - 3 targetFile 加密的后文件所存的位置
        /// - 4 jsOptions  需要加密的数据可选参数
        ///     - 1 CryptAlgorithm        加密算法，默认des
        ///     - 2 EncodeDataType        数据编码格式，默认base64
        ///     - 3 kKeyForEncodeKeyType  密钥编码格式，默认16进制
        public void encryptFile(string options)
        {
            DoFileCrypt(true, options);
        }

        /// <summary>
        /// 使用指定算法解密文件,通过回调返回解密后的数据.
        /// </summary>
        /// <param name="options"></param>
        /// command.arguments:
        /// - 1 sKey 密钥
        /// - 2 sourceFile 需要解密的文件
        /// - 3 targetFile 解密的后文件所存的位置
        /// - 4 jsOptions  需要解密的数据可选参数
        ///     - 1 CryptAlgorithm        加密算法，默认des
        ///     - 2 EncodeDataType        数据编码格式，默认base64
        ///     - 3 kKeyForEncodeKeyType  密钥编码格式，默认16进制
        public void decryptFile(string options)
        {
            DoFileCrypt(false, options);
        }

        /// <summary>
        /// 根据传入的数据求MD5值，并通过回调返回该数据的MD5值
        /// </summary>
        /// <param name="options"></param>
        /// command.arguments:
        /// - 1 data 需要求MD5值的数据
        public void digest(string options)
        {
            string dataEncryptStr = string.Empty;
            string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
            string sourceContent = args[0];

            byte[] sourceData = Encoding.UTF8.GetBytes(sourceContent);
            byte[] result = DigestUtilities.CalculateDigest("MD5", sourceData);
            dataEncryptStr = BitConverter.ToString(result);
            dataEncryptStr = dataEncryptStr.Replace("-", "");//去掉"-" 和 ios的结果保持一致
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, dataEncryptStr));
        }

        private byte[] StringToData(string input, EncodeType type)
        {
            if (EncodeType.XDataBase64Encoding == type)
            {
                return Convert.FromBase64String(input);
            }
            if (EncodeType.XDataHexEncoding == type)
            {
                input = input.Replace(" ", "");
                if ((input.Length % 2) != 0)
                    input += " ";
                byte[] returnBytes = new byte[input.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);
                return returnBytes;
            }
            return Encoding.UTF8.GetBytes(input);
        }

        private string DataToString(byte[] input, EncodeType type)
        {
            if (EncodeType.XDataBase64Encoding == type)
            {
                return Convert.ToBase64String(input);
            }
            if (EncodeType.XDataHexEncoding == type)
            {
                string returnStr = "";
                if (input != null)
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        returnStr += input[i].ToString("X2");
                    }
                }
                return returnStr;
            }
            return Encoding.UTF8.GetString(input, 0, input.Length);
        }

        private byte[] Encrypt(byte[] keyData, byte[] sourceData, CryptAlgorithm alg)
        {
            byte[] result = null;
            KeyParameter keyParam = null;
            IBufferedCipher engine = null;
            switch (alg)
            {
                case CryptAlgorithm.TRIPLE_DES:
                    keyParam = new DesEdeParameters(keyData);
                    engine = CipherUtilities.GetCipher("DESede/CBC/PKCS7PADDING");
                    engine.Init(true, keyParam);
                    result = engine.DoFinal(sourceData);
                    break;
                case CryptAlgorithm.RSA:
                    throw new NotSupportedException("CryptAlgorithm.RSA NOT SUPPORT YET");
                case CryptAlgorithm.DES:
                default://默认DES
                    keyParam = new DesParameters(keyData);
                    engine = CipherUtilities.GetCipher("DES/ECB/PKCS7PADDING");
                    engine.Init(true, keyParam);
                    result = engine.DoFinal(sourceData);
                    break;
            }

            return result;
        }

        private byte[] Decrypt(byte[] keyData, byte[] sourceData, CryptAlgorithm alg)
        {
            byte[] result = null;
            KeyParameter keyParam = null;
            IBufferedCipher engine = null;
            switch (alg)
            {
                case CryptAlgorithm.TRIPLE_DES:
                    keyParam = new DesEdeParameters(keyData);
                    engine = CipherUtilities.GetCipher("DESede/CBC/PKCS7PADDING");
                    engine.Init(false, keyParam);
                    result = engine.DoFinal(sourceData);
                    break;
                case CryptAlgorithm.RSA:
                    throw new NotSupportedException("CryptAlgorithm.RSA NOT SUPPORT YET");
                case CryptAlgorithm.DES:
                default://默认DES
                    keyParam = new DesParameters(keyData);
                    engine = CipherUtilities.GetCipher("DES/ECB/PKCS7PADDING");
                    engine.Init(false, keyParam);
                    result = engine.DoFinal(sourceData);
                    break;
            }

            return result;
        }


        //根据加密算法 截取 keyData
        //DES 密钥要求为8位 3DES的密钥要求为3*8
        private byte[] MakeKey(byte[] keyData, CryptAlgorithm alg)
        {
            byte[] key = keyData;
            if (keyData.Length > 8)
            {
                if (CryptAlgorithm.DES == alg)
                {
                    key = new byte[8];
                    Array.Copy(keyData, 0, key, 0, 8);
                }
                if (CryptAlgorithm.TRIPLE_DES == alg)
                {
                    if (keyData.Length > 24)
                    {
                        key = new byte[24];
                        Array.Copy(keyData, 0, key, 0, 24);
                    }
                }
            }
            return key;
        }

        private void DoFileCrypt(bool encrypt, string options)
        {
            string sKey = string.Empty;
            string sourceFilePath = string.Empty;
            string targetFilePath = string.Empty;

            string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
            sKey = args[0];
            sourceFilePath = args[1];
            targetFilePath = args[2];
            SecurityOptions option = new SecurityOptions();
            if (args.Length > 3 && null != args[3])
            {
                option = JSON.JsonHelper.Deserialize<SecurityOptions>(args[3]);
            }

            //check
            if (string.IsNullOrEmpty(sKey) || sKey.Length < 8)
            {
                XLog.WriteError("Input data invalid!");
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                return;
            }

            //path_error
            if (null == sourceFilePath || null == targetFilePath)
            {
                XLog.WriteError("Input data invalid path error!");
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, SecurityError.PATH_ERR));
                return;
            }
            //资源文件不存在
            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoFile.FileExists(sourceFilePath))
                {
                    XLog.WriteError(sourceFilePath + ": not_found_error!");
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, SecurityError.FILE_NOT_FOUND_ERR));
                    return;
                }
            }

            try
            {
                var keyData = StringToData(sKey, option.EncodeKeyType);
                keyData = MakeKey(keyData, option.CryptAlgorithm);
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (Stream sourceStream = isoFile.OpenFile(sourceFilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (FileStream fileStream = new IsolatedStorageFileStream(targetFilePath, FileMode.OpenOrCreate, isoFile))
                        {
                            using (BinaryWriter writer = new BinaryWriter(fileStream))
                            {
                                int streamLength = (int)sourceStream.Length;
                                byte[] fileData = new byte[streamLength];
                                sourceStream.Read(fileData, 0, streamLength);
                                byte[] result = null;
                                if (encrypt)
                                {
                                    result = Encrypt(keyData, fileData, option.CryptAlgorithm);
                                }
                                else
                                {
                                    result = Decrypt(keyData, fileData, option.CryptAlgorithm);
                                }
                                writer.Write(result);
                            }
                        }
                    }

                }
                //返回加密后文件路径
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, targetFilePath));

            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is ArgumentNullException || ex is ArgumentException || ex is ArgumentOutOfRangeException || ex is NotSupportedException)
                {
                    string operation = encrypt ? "encrypt" : "decrypt";
                    XLog.WriteError("DoFileCrypt " + operation + " error with Exception : " + ex.Message);
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, SecurityError.OPERATION_ERR));
                    return;
                }
                throw ex;
            }
        }
    }
}
