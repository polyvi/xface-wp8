using System;

namespace xFaceLib.Util
{
    public class XConstant
    {
        /// <summary>
        ///  Ĭ��buffer����
        /// </summary>
        public const int BUFFER_LEN = 2048;

        /// <summary>
        ///  һ���ڵĺ�����
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
        ///  ��ȡԤ�ð�sms scheme
        /// </summary>
        public const String SCHEME_SMS = "sms:";

        /// <summary>
        ///  app��Ĭ������ҳ��
        /// </summary>
        public const String DEFAULT_START_PAGE_NAME = "index.html";

        /// <summary>
        /// playerģʽ Ĭ��APPID
        /// </summary>
        public const string DEFAULT_APP_ID_FOR_PLAYER = "app";

        /// <summary>
        /// xapp������
        /// </summary>
        public const string APP_TYPE_XAPP = "xapp";

        /// <summary>
        /// napp������
        /// </summary>
        public const string APP_TYPE_NAPP = "napp";

        /// <summary>
        ///  portal��װ�ɹ�
        /// </summary>
        public const int PORTAL_INSTALL_SUCCESS = 1;

        /// <summary>
        ///  portal��װʧ��
        /// </summary>
        public const int PORTAL_INSTALL_FAIL = 0;

        /// <summary>
        ///  assetsĿ¼����Ҫ��ѹ��zip����
        /// </summary>
        public const String ASSET_PACKAGE_FILE_NAME = "xFaceInstalledPackage.zip";

        /// <summary>
        ///  ϵͳ�����ļ�����
        /// </summary>
        public const String CONFIG_FILE_NAME = "config.xml";

        /// <summary>
        ///  Ԥ��װ��app����Ŀ¼����
        /// </summary>
        public const String PREINSTALL_PACKAGE_DIR_NAME = "pre_install";

        /// <summary>
        ///  Ĭ��app����
        /// </summary>
        public const String APP_TYPE_DEFAULT = "app";

        /// <summary>
        ///  ���Ӧ������
        /// </summary>
        public const String APP_TYPE_COMPONENT = "component";

        /// <summary>
        ///  Ӧ�õ������ļ�����
        /// </summary>
        public const String APP_CONFIG_FILE_NAME = "app.xml";

        /// <summary>
        ///  Ӧ�õĹ���Ŀ¼����
        /// </summary>
        public const String APP_WORK_DIR_NAME = "workspace";

        /// <summary>
        ///  �洢Ӧ�����ݵ�Ŀ¼����
        /// </summary>
        public const String APP_DATA_DIR_NAME = "data";

        /// <summary>
        ///  ����Ӧ�û���·��
        /// </summary>
        public const String APP_CACHE_PATH = "app_cache";

        /// <summary>
        ///  Ԥ��Ӧ�ð�Ŀ¼����
        /// </summary>
        public const String PRE_SET_APP_PACKAGE_DIR_NAME = "pre_set";

        /// <summary>
        ///  �������ݰ�Ŀ¼����
        /// </summary>
        public const String ENCRYPT_CODE_DIR_NAME = "encrypt_code";

        /// <summary>
        ///  ��װ���ĺ�׺��
        /// </summary>
        public const String APP_PACKAGE_SUFFIX = ".zip";

        /// <summary>
        ///  native��װ���ĺ�׺��
        /// </summary>
        public const String NATIVE_APP_SUFFIX_NPA = ".npa";

        /// <summary>
        ///  ��װ���ĺ�׺��,��ɢ�ļ���ʽ
        /// </summary>
        public const String APP_PACKAGE_SUFFIX_XPA = ".xpa";

        /// <summary>
        /// ��װ���ĺ�׺��,SingleFile��ʽ
        /// </summary>
        public const String APP_PACKAGE_SUFFIX_XSPA = ".xspa";

        /// <summary>
        ///  js���ʵ���ļ����ļ���
        /// </summary>
        public const String XFACE_JS_FILE_NAME = "xface.js";

        /// <summary>
        ///  js���������ļ�
        /// </summary>
        public const String DEBUG_JS_FILE_NAME = "xdebug.js";

        /// <summary>
        ///  Ĭ�ϴ����ļ���
        /// </summary>
        public const String ERROR_PAGE_NAME = "xFaceError.html";

        /// <summary>
        ///  app��������
        /// </summary>
        public const String TAG_APP_START_PARAMS = "start_params";

        /// <summary>
        ///  �ַ�ͨ���
        /// </summary>
        public const String WILDCARDS = "*";

        /// <summary>
        ///  ���patch����������patch������PATCH_NAME��Version���
        /// </summary>
        public const String PATCH_NAME = "patch";

        /// <summary>
        ///  �����װ���ĺ�׺
        /// </summary>
        public const String PACKAGE_SUFFIX = ".zip";

        public const String TAG_COMPONENTS = "components";
        public const String TAG_COMPONENT = "component";
        public const String ATTR_VERSION = "version";
        public const String ATTR_SERVER = "server";
        public const String ATTR_IS_UPDATE = "isUpdated";
        public const String BASE_NAME = "base";

        public const String PRE_INSTALL_SOURCE_ROOT = "Assets\\data\\";
        /// <summary>
        ///  Ӧ���ڹ���Ŀ¼���������ݰ���
        /// </summary>
        public const String APP_DATA_PACKAGE_NAME_IN_WORKSAPCE = "workspace.zip";

        public const String PUSH_NOTIFICATION_URI = "push_notification_uri";

        /// <summary>
        /// ϵͳ״̬�� �����߶�
        /// </summary>
        public const int SYSTEMTRAY_HEIGHT = 32;
    }
}
