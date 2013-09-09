using System;
using xFaceLib.runtime;
using xFaceLib.ams;

namespace xFaceLib.extensions.ams
{
    /// <summary>
    /// 封装ams的各种操作，主要包括app的基本操作，设置监听器等功能，以提供给extension使用
    /// </summary>
    public interface XAms
    {
        /// <summary>
        /// 启动一个应用
        /// </summary>
        /// <param name="appId">应用的id</param>
        /// <param name="appparams">启动程序参数</param>
        /// <returns>启动成功返回ture，失败返回false</returns>
        bool StartApp(String appId, String appparams);

        /// <summary>
        /// 安装一个应用
        /// </summary>
        /// <param name="path">应用安装包的相对路径</param>
        void InstallApp(String path, XAppInstallListener listener);

        /// <summary>
        /// 更新一个应用
        /// </summary>
        /// <param name="path">应用安装包的相对路径</param>
        void UpdateApp(String path, XAppInstallListener listener);

        /// <summary>
        /// 卸载应用
        /// </summary>
        /// <param name="appId">应用的id</param>
        void UninstallApp(String appId, XAppInstallListener listener);

        /// <summary>
        /// 关闭应用
        /// </summary>
        /// <param name="appId">应用的id</param>
        void CloseApp(String appId);

        /// <summary>
        /// 获取应用列表
        /// </summary>
        /// <returns>应用列表</returns>
        XApplicationList GetAppList();

        /// <summary>
        /// app是否能够执行ams能力
        /// </summary>
        /// <param name="app">调用该接口的应用对象</param>
        /// <returns>能够执行ams返回ture，否则返回false</returns>
        bool CanExecuteAmsBy(XApplication app);

        /// <summary>
        ///  获取预置包
        /// </summary>
        /// <returns>返回预置包数组，每一项是预置包名</returns>
        String[] GetPresetAppPackages();

        /// <summary>
        /// 获取默认应用的描述信息
        /// </summary>
        /// <returns>默认应用的描述信息</returns>
        XAppInfo GetDefaultAppInfo();
    }
}
