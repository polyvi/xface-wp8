using System;

namespace xFaceLib.runtime
{
    public class XApplicationCreator
    {
        private static String XFACE_APP_XAPP = "xapp";

        public static String NATIVE_APP_NAPP = "napp";

        public static String XFACE_APP_APP = "app";

        public static XApplication Create(XAppInfo info)
        {
            if (info.Type.Equals(XFACE_APP_XAPP) || info.Type.Equals(XFACE_APP_APP))
            {
                XWebApplication app = new XWebApplication(info);
                return app;
            }
            else if (info.Type.Equals(NATIVE_APP_NAPP))
            {
                XNativeApplication app = new XNativeApplication(info);
                return app;
            }
            return null;
        }
    }
}
