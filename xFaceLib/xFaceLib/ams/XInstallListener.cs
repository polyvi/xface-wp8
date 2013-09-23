using System;
using System.Text;
using System.Threading.Tasks;
using xFaceLib.runtime;

namespace xFaceLib.ams
{
    /// <summary>
    /// ״̬�����
    /// </summary>
    public enum InstallStatus : int
    {
        INSTALL_INITIALIZE,             /** ��װ��ʼ�� */
        INSTALL_UNZIP_PACKAGE,          /** ������� */
        INSTALL_WRITE_CONFIGURATION,    /** д���õĹ��� */
        INSTALL_FINISHED,               /** ��װ��� */
    };

    /// <summary>
    /// Ӧ�ð�װ/ж��/���²���������
    /// </summary>
    public enum AMS_ERROR : int
    {
        ERROR_BASE,             /** not used */
        NO_SRC_PACKAGE,         /** Ӧ�ð�װ�������� */
        APP_ALREADY_EXISTED,    /** Ӧ���Ѿ����� */
        IO_ERROR,               /**IO �쳣���� */
        NO_TARGET_APP,          /** û���ҵ���������Ŀ��Ӧ�� */
        NO_APP_CONFIG_FILE,     /** ������Ӧ�������ļ� */
        RESERVED,               /** �����ֶ�, ���ݾɵ�REMOVE_APP_FAILED*/
        UNKNOWN,                /** δ֪���� */
    };

    /// <summary>
    /// ams��������
    /// </summary>
    public enum AMS_OPERATION_TYPE : int
    {
        OPERATION_NONE,
        OPERATION_TYPE_INSTALL,     /** ��װ�������� */
        OPERATION_TYPE_UPDATE,      /** ���²������� */
        OPERATION_TYPE_UNINSTALL,   /** ж�ز������� */
    };

    /// <summary>
    /// ����Ӧ�ð�װ���ȼ����������ӿڣ�����װ���ȼ�״̬֪ͨ
    /// </summary>
    public abstract class XInstallListener
    {
        /// <summary>
        /// ���°�װ����
        /// </summary>
        /// <param name="type">���ͱ�ʶ����װ/ж��</param>
        /// <param name="progressState">����״̬</param>
        public abstract void OnProgressUpdated(AMS_OPERATION_TYPE type, InstallStatus progressState);

        /// <summary>
        /// ��װ����ص�
        /// </summary>
        /// <param name="type">���ͱ�ʶ����װ/ж��</param>
        /// <param name="appId">Ӧ��id</param>
        /// <param name="errorState">������</param>
        public abstract void OnError(AMS_OPERATION_TYPE type, String appId, AMS_ERROR errorState);

        /// <summary>
        /// ��װ�ɹ��ص�
        /// </summary>
        /// <param name="type">���ͱ�ʶ����װ/ж��</param>
        /// <param name="appId">Ӧ��id</param>
        public abstract void OnSuccess(AMS_OPERATION_TYPE type, String appId);
    }
}
