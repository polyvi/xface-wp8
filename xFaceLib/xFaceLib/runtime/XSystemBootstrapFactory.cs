using System;
using System.Reflection;

namespace xFaceLib.runtime
{
    public class XSystemBootstrapFactory
    {
        public static XSystemBootstrap CreateSystemBootstrap()
        {
            Type t = null;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = a.GetType("xFaceLib.runtime.XPlayerSystemBootstrap");
                if (t != null)
                {
                    break;
                }
            }
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