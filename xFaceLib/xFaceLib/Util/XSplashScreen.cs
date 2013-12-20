using System;
using System.IO;
using System.Windows.Controls.Primitives;
using xFaceLib.SplashScreenControl;
using System.Xml;
using System.Xml.Linq;
using xFaceLib.runtime;

namespace xFaceLib.Util
{
    public class XSplashScreen
    {
        /// <summary>
        /// Popup控件
        /// </summary>
        private Popup popup;

        private static XSplashScreen instance;

        public static XSplashScreen GetInstance()
        {
            if (null == instance)
            {
                instance = new XSplashScreen();
            }
            return instance;
        }

        /// <summary>
        /// 显示xFace splash界面
        /// </summary>
        public void ShowxFaceSplash()
        {
            if (null == this.popup)
            {
                this.popup = new Popup();
            }

            if (this.popup.IsOpen)
            {
                return;
            }
            var splash = new XSplashScreenControl();

            XSystemBootstrap boot = XSystemBootstrapFactory.CreateSystemBootstrap();
            if (boot.GetType().ToString().Equals("xFaceLib.runtime.XPlayerSystemBootstrap"))
            {
                string version = XSystemConfiguration.GetInstance().XFaceVersion + " " + XSystemConfiguration.GetInstance().BuildNumber;
                string product = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Description").Value;
                splash.SetDisplayInfo("version: " + version, "Product: " + product);
            }

            DetermineSplashImage(splash);
            this.popup.Child = splash;
            this.popup.IsOpen = true;
        }

        /// <summary>
        /// 显示应用splash
        /// </summary>
        /// <param name="imagePath">指定显示splash的图片路径</param>
        public bool ShowAppSplash(string imagePath)
        {
            if (null == this.popup)
            {
                this.popup = new Popup();
            }

            if (this.popup.IsOpen)
            {
                return false;
            }

            var splash = new XSplashScreenControl();
            //文件不存在 使用默认splash
            if (!File.Exists(imagePath))
            {
                DetermineSplashImage(splash);
            }
            else
            {
                //FIXME:用1张图片不能覆盖3个分辨率，如果要精确最好是不同分辨率的图片
                try
                {
                    splash.SetSplashImage(imagePath);
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (UriFormatException)
                {
                    return false;
                }
            }

            this.popup.Child = splash;
            this.popup.IsOpen = true;
            return true;
        }

        /// <summary>
        /// 关闭splash
        /// </summary>
        public void Hide()
        {
            if (null == this.popup)
            {
                return;
            }
            this.popup.IsOpen = false;
        }

        /// <summary>
        /// 根据设备分辨率决定splashImage
        /// </summary>
        /// <param name="splash">splash控件</param>
        private void DetermineSplashImage(XSplashScreenControl splash)
        {
            switch (XResolutionHelper.CurrentResolution)
            {
                case Resolutions.HD1080p:
                    splash.SetSplashImage("../SplashScreenImage-1080p.jpg");
                    break;
                case Resolutions.HD720p:
                    splash.SetSplashImage("../SplashScreenImage-720p.jpg");
                    break;
                case Resolutions.WVGA:
                    splash.SetSplashImage("../SplashScreenImage-WVGA.jpg");
                    break;
                case Resolutions.WXGA:
                    splash.SetSplashImage("../SplashScreenImage-WXGA.jpg");
                    break;
                default:
                    splash.SetSplashImage("../SplashScreenImage-1080p.jpg");
                    break;
            }
        }
    }
}
