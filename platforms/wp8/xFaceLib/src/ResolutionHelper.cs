﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xFaceLib
{
    public enum Resolutions { WVGA, WXGA, HD720p };

    public static class ResolutionHelper
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

        public static Resolutions CurrentResolution
        {
            get
            {
                if (IsWvga) return Resolutions.WVGA;
                else if (IsWxga) return Resolutions.WXGA;
                else if (Is720p) return Resolutions.HD720p;
                else throw new InvalidOperationException("Unknown resolution");
            }
        }
    }
}
