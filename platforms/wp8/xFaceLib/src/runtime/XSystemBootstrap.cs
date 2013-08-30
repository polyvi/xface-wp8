using System;
using Windows.Storage;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Resources;
using xFaceLib.Util;
using xFaceLib.ams;

namespace xFaceLib.runtime
{
    public class XSystemBootstrap
    {
        /// <summary>
        /// 工作环境准备失败的事件
        /// </summary>
        public event EventHandler<string> FailToPrepareWorkEnvironment;

        /// <summary>
        /// 工作环境准备完成的事件
        /// </summary>
        public event EventHandler<string> FinishToPrepareWorkEnvironment;

        /// <summary>
        /// 启动之前的准备工作
        /// </summary>
        public virtual void PrepareWorkEnvironment()
        { }

        /// <summary>
        /// 1. 解析系统的配置文件
        /// 2. 根据配置文件进行预安装
        /// 3. 预安装完后启动默认的app
        /// </summary>
        /// <param name="runtime"></param>
        public virtual void Boot(XAppManagement appManagement)
        { }

        public void FireFinishToPrepareWorkEnvironment(string result)
        {
            this.FinishToPrepareWorkEnvironment(this, result);
        }

        public void FireFailToPrepareWorkEnvironment(string result)
        {
            this.FailToPrepareWorkEnvironment(this, result);
        }
   
    }
}
