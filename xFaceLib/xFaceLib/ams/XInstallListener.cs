using System;
using System.Text;
using System.Threading.Tasks;
using xFaceLib.runtime;

namespace xFaceLib.ams
{
    /// <summary>
    /// 状态的类别
    /// </summary>
    public enum InstallStatus : int
    {
        INSTALL_INITIALIZE,             /** 安装初始化 */
        INSTALL_UNZIP_PACKAGE,          /** 解包过程 */
        INSTALL_WRITE_CONFIGURATION,    /** 写配置的过程 */
        INSTALL_FINISHED,               /** 安装完成 */
    };

    /// <summary>
    /// 应用安装/卸载/更新操作错误码
    /// </summary>
    public enum AMS_ERROR : int
    {
        ERROR_BASE,             /** not used */
        NO_SRC_PACKAGE,         /** 应用安装包不存在 */
        APP_ALREADY_EXISTED,    /** 应用已经存在 */
        IO_ERROR,               /**IO 异常错误 */
        NO_TARGET_APP,          /** 没有找到待操作的目标应用 */
        NO_APP_CONFIG_FILE,     /** 不存在应用配置文件 */
        RESERVED,               /** 保留字段, 兼容旧的REMOVE_APP_FAILED*/
        UNKNOWN,                /** 未知错误 */
    };

    /// <summary>
    /// ams操作类型
    /// </summary>
    public enum AMS_OPERATION_TYPE : int
    {
        OPERATION_NONE,
        OPERATION_TYPE_INSTALL,     /** 安装操作类型 */
        OPERATION_TYPE_UPDATE,      /** 更新操作类型 */
        OPERATION_TYPE_UNINSTALL,   /** 卸载操作类型 */
    };

    /// <summary>
    /// 定义应用安装进度监听器基本接口，负责安装进度及状态通知
    /// </summary>
    public abstract class XInstallListener
    {
        /// <summary>
        /// 更新安装进度
        /// </summary>
        /// <param name="type">类型标识：安装/卸载</param>
        /// <param name="progressState">进度状态</param>
        public abstract void OnProgressUpdated(AMS_OPERATION_TYPE type, InstallStatus progressState);

        /// <summary>
        /// 安装错误回调
        /// </summary>
        /// <param name="type">类型标识：安装/卸载</param>
        /// <param name="appId">应用id</param>
        /// <param name="errorState">错误码</param>
        public abstract void OnError(AMS_OPERATION_TYPE type, String appId, AMS_ERROR errorState);

        /// <summary>
        /// 安装成功回调
        /// </summary>
        /// <param name="type">类型标识：安装/卸载</param>
        /// <param name="appId">应用id</param>
        public abstract void OnSuccess(AMS_OPERATION_TYPE type, String appId);
    }
}
