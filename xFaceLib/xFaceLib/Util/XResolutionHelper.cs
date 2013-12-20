using System;

namespace xFaceLib.Util
{
    public enum Resolutions { WVGA, WXGA, HD720p, HD1080p };

    public static class XResolutionHelper
    {
        private static bool IsWvga
        {
            get
            {
                return System.Windows.Application.Current.Host.Content.ScaleFactor == 100;
            }
        }

        private static bool IsWxga
        {
            get
            {
                return System.Windows.Application.Current.Host.Content.ScaleFactor == 160;
            }
        }

        private static bool Is720p
        {
            get
            {
                return System.Windows.Application.Current.Host.Content.ScaleFactor == 150;
            }
        }

        private static bool Is1080p
        {
            get
            {
                return System.Windows.Application.Current.Host.Content.ScaleFactor == 225;
            }
        }

        public static Resolutions CurrentResolution
        {
            get
            {
                if (IsWvga) return Resolutions.WVGA;
                else if (IsWxga) return Resolutions.WXGA;
                else if (Is720p) return Resolutions.HD720p;
                else if (Is1080p) return Resolutions.HD1080p;
                else throw new InvalidOperationException("Unknown resolution");
            }
        }
    }
}
