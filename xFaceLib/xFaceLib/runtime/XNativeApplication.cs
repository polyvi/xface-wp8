using System;
using xFaceLib.ams;


namespace xFaceLib.runtime
{
    public class XNativeApplication : XApplication
    {
        public XNativeApplication(XAppInfo applicationInfo)
            : base(applicationInfo)
        {

        }

        /// <summary>
        /// app 加载应用启动
        /// </summary>
        public override async void Load()
        {
            //FIXME：目前无法判断应用是否已安装，故每次直接调用加载应用，如未安装系统会去查找；
            //remote-pkg所设置的参数暂未使用
            String url = AppInfo.Entry;
            bool ret = await Windows.System.Launcher.LaunchUriAsync(new System.Uri(url));
        }
    }
}
