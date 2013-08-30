using System;
using System.IO;
using xFaceLib.runtime;
using xFaceLib.Util;
using System.IO.IsolatedStorage;
using xFaceLib.Log;

namespace xFaceLib.ams
{
    /// <summary>
    /// Ԥװ�����������ڼ���Ԥװ��װ����
    /// </summary>
    public class XPreInstallListener
    {
        /// <summary>
        /// app ������
        /// </summary>
        private XAppManagement ams;

        public XPreInstallListener(XAppManagement ams)
        {
            this.ams = ams;
        }

        /// <summary>
        /// ��װʧ�ܱ�����
        /// </summary>
        public void onFailure()
        {
            XLog.WriteError("PreInstall error! ");
        }

        /**
        * ��װ�ɹ� �ص�������
        * @param startApp ����app��id
        */
        /// <summary>
        /// ��װ�ɹ�������
        /// </summary>
        public void OnSuccess()
        {
            ams.StartDefaultApp(null);
        }        
    }
}