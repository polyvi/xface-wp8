using System;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;

namespace xFaceLib.runtime
{
    public class XStartParams
    {
        private const String START_PARAMS = "StartParams";

        public string PageEntry { get; set; }
        public string Data { get; set; }

        private XStartParams(string page, string data)
        {
            this.PageEntry = page;
            this.Data = data;
        }

        /// <summary>
        /// 解析启动参数 启动参数格式:startpage=a/b.html;data=...
        /// </summary>
        /// <param name="startparams"> 启动参数 </param>
        /// <returns>新建的XStartParams对象</returns>
        public static XStartParams Parse(string startparams)
        {
            if (startparams == null || startparams == string.Empty)
            {
                return null;
            }

            string page = null;
            string data = null;

            string[] arrParams = startparams.Split(new Char[] { ';'}, 2);
            for (var i = 0; i < arrParams.Length; i++)
            {
                string[] split = arrParams[i].Split('=');
                if (split.Length > 1)
                {
                    string key = split[0];
                    string val = split[1];
                    switch (key)
                    {
                        case "startpage":
                            page = val;
                            break;
                        case "data":
                            data = val;
                            break;
                        default:
                            break;
                    }
                }
            }
            if (null == page && null == data)
            {
                data = startparams;
            }
            return new XStartParams(page, data);
        }

        /// <summary>
        /// 将启动参数存到配置文件中
        /// <param name="param"> 启动参数 </param>
        /// </summary>
        public static void SaveStartParams(string param)
        {
            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
            userSettings[START_PARAMS] = param;
            userSettings.Save();
        }

        /// <summary>
        /// 获取存储到配置文件中的启动参数,并将其清除
        /// </summary>
        /// <returns>XStartParams对象</returns>
        public static string GetStartParams()
        {
            string StartParams = null;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(START_PARAMS))
            {
                StartParams = (string)IsolatedStorageSettings.ApplicationSettings[START_PARAMS];
                IsolatedStorageSettings.ApplicationSettings[START_PARAMS] = null;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            return StartParams;
        }
    }

}
