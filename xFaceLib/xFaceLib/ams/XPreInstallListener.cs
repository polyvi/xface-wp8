using System;
using System.IO;
using xFaceLib.runtime;
using xFaceLib.Util;
using System.IO.IsolatedStorage;
using xFaceLib.Log;

namespace xFaceLib.ams
{
    /// <summary>
    /// 预装监听器，用于监听预装安装过程
    /// </summary>
    public class XPreInstallListener
    {
        /// <summary>
        /// app 管理器
        /// </summary>
        private XAppManagement ams;

        public XPreInstallListener(XAppManagement ams)
        {
            this.ams = ams;
        }

        /// <summary>
        /// 安装失败被调用
        /// </summary>
        public void onFailure()
        {
            XLog.WriteError("PreInstall error! ");
        }

        /// <summary>
        /// 安装成功被调用
        /// </summary>
        public void OnSuccess()
        {
            string StartParams = XStartParams.GetStartParams();
            ams.StartDefaultApp(XStartParams.Parse(StartParams));
        }
    }
}