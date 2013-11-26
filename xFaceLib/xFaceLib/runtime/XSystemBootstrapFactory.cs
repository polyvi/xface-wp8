using System;

namespace xFaceLib.runtime
{
    public class XSystemBootstrapFactory
    {
        public static XSystemBootstrap CreateSystemBootstrap()
        {
            Type t = Type.GetType("xFaceLib.runtime.XPlayerSystemBootstrap");
            if (null == t)
            {
                return new XGeneralSystemBootstrap();
            }
            else
            {
                return Activator.CreateInstance(t) as XSystemBootstrap;
            }
        }
    }
}