using System;

namespace xFaceLib.runtime
{
    public interface XAmsDelegate
    {

        /// <summary>
        /// 启动application
        /// </summary>
        /// <param name="app">待启动的application</param>
        void StartApp(XWebApplication app);

        /// <summary>
        /// 关闭application
        /// </summary>
        /// <param name="app">待关闭的application</param>
        void CloseApp(XWebApplication app);

    }
}
