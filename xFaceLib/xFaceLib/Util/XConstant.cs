using System;

namespace xFaceLib.Util
{
    public class XConstant
    {
        /// <summary>
        ///  默认buffer长度
        /// </summary>
        public const int BUFFER_LEN = 2048;

        /// <summary>
        ///  一秒内的毫秒数
        /// </summary>
        public const int MILLISECONDS_PER_SECOND = 1000;

        /// <summary>
        ///  file scheme
        /// </summary>
        public const String FILE_SCHEME = "file://";

        /// <summary>
        ///  http scheme
        /// </summary>
        public const String HTTP_SCHEME = "http://";

        /// <summary>
        ///  https scheme
        /// </summary>
        public const String HTTPS_SCHEME = "https://";

        /// <summary>
        ///  content scheme
        /// </summary>
        public const String CONTENT_SCHEME = "content:";

        /// <summary>
        ///  获取预置包sms scheme
        /// </summary>
        public const String SCHEME_SMS = "sms:";

        /// <summary>
        ///  app的默认启动页面
        /// </summary>
        public const String DEFAULT_START_PAGE_NAME = "index.html";

        /// <summary>
        /// player模式 默认APPID
        /// </summary>
        public const string DEFAULT_APP_ID_FOR_PLAYER = "app";

        /// <summary>
        /// xapp包类型
        /// </summary>
        public const string APP_TYPE_XAPP = "xapp";

        /// <summary>
        /// napp包类型
        /// </summary>
        public const string APP_TYPE_NAPP = "napp";

        /// <summary>
        ///  portal安装成功
        /// </summary>
        public const int PORTAL_INSTALL_SUCCESS = 1;

        /// <summary>
        ///  portal安装失败
        /// </summary>
        public const int PORTAL_INSTALL_FAIL = 0;

        /// <summary>
        ///  assets目录下需要解压的zip包名
        /// </summary>
        public const String ASSET_PACKAGE_FILE_NAME = "xFaceInstalledPackage.zip";

        /// <summary>
        ///  系统配置文件名称
        /// </summary>
        public const String CONFIG_FILE_NAME = "config.xml";

        /// <summary>
        ///  预安装的app所在目录名称
        /// </summary>
        public const String PREINSTALL_PACKAGE_DIR_NAME = "pre_install";

        /// <summary>
        ///  默认app类型
        /// </summary>
        public const String APP_TYPE_DEFAULT = "app";

        /// <summary>
        ///  组件应用类型
        /// </summary>
        public const String APP_TYPE_COMPONENT = "component";

        /// <summary>
        ///  应用的配置文件名称
        /// </summary>
        public const String APP_CONFIG_FILE_NAME = "app.xml";

        /// <summary>
        ///  应用的工作目录名称
        /// </summary>
        public const String APP_WORK_DIR_NAME = "workspace";

        /// <summary>
        ///  存储应用数据的目录名称
        /// </summary>
        public const String APP_DATA_DIR_NAME = "data";

        /// <summary>
        ///  离线应用缓存路径
        /// </summary>
        public const String APP_CACHE_PATH = "app_cache";

        /// <summary>
        ///  预置应用包目录名称
        /// </summary>
        public const String PRE_SET_APP_PACKAGE_DIR_NAME = "pre_set";

        /// <summary>
        ///  加密数据包目录名称
        /// </summary>
        public const String ENCRYPT_CODE_DIR_NAME = "encrypt_code";

        /// <summary>
        ///  安装包的后缀名
        /// </summary>
        public const String APP_PACKAGE_SUFFIX = ".zip";

        /// <summary>
        ///  native安装包的后缀名
        /// </summary>
        public const String NATIVE_APP_SUFFIX_NPA = ".npa";

        /// <summary>
        ///  安装包的后缀名,离散文件方式
        /// </summary>
        public const String APP_PACKAGE_SUFFIX_XPA = ".xpa";

        /// <summary>
        /// 安装包的后缀名,SingleFile方式
        /// </summary>
        public const String APP_PACKAGE_SUFFIX_XSPA = ".xspa";

        /// <summary>
        ///  js框架实现文件的文件名
        /// </summary>
        public const String XFACE_JS_FILE_NAME = "xface.js";

        /// <summary>
        ///  js调试配置文件
        /// </summary>
        public const String DEBUG_JS_FILE_NAME = "xdebug.js";

        /// <summary>
        ///  默认错误文件名
        /// </summary>
        public const String ERROR_PAGE_NAME = "xFaceError.html";

        /// <summary>
        ///  app启动参数
        /// </summary>
        public const String TAG_APP_START_PARAMS = "start_params";

        /// <summary>
        ///  字符通配符
        /// </summary>
        public const String WILDCARDS = "*";

        /// <summary>
        ///  组件patch包基础名，patch包名有PATCH_NAME和Version组成
        /// </summary>
        public const String PATCH_NAME = "patch";

        /// <summary>
        ///  组件安装包的后缀
        /// </summary>
        public const String PACKAGE_SUFFIX = ".zip";

        public const String TAG_COMPONENTS = "components";
        public const String TAG_COMPONENT = "component";
        public const String ATTR_VERSION = "version";
        public const String ATTR_SERVER = "server";
        public const String ATTR_IS_UPDATE = "isUpdated";
        public const String BASE_NAME = "base";

        public const String PRE_INSTALL_SOURCE_ROOT = "xface3\\";
        /// <summary>
        ///  应用在工作目录中内置数据包名
        /// </summary>
        public const String APP_DATA_PACKAGE_NAME_IN_WORKSAPCE = "workspace.zip";

        public const String PUSH_NOTIFICATION_URI = "push_notification_uri";

        /// <summary>
        /// 系统状态栏 竖屏高度
        /// </summary>
        public const int SYSTEMTRAY_HEIGHT = 32;
    }
}
