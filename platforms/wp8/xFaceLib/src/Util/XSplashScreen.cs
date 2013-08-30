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
            if(XSystemConfiguration.GetInstance().IsPlayerMode)
            {
                string version = XSystemConfiguration.GetInstance().XFaceVersion + " " + XSystemConfiguration.GetInstance().BuildNumber;
                string product = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Description").Value;
                splash.SetDisplayInfo("version: "+version, "Product: "+product);
            }

            switch (XResolutionHelper.CurrentResolution)
            {
                case Resolutions.HD720p:
                    splash.SetSplashImage("../SplashScreenImage.screen-720p.jpg");
                    break;
                case Resolutions.WVGA:
                    splash.SetSplashImage("../SplashScreenImage.screen-WVGA.jpg");
                    break;
                case Resolutions.WXGA:
                    splash.SetSplashImage("../SplashScreenImage.screen-WXGA.jpg");
                    break;
                default:
                    splash.SetSplashImage("../SplashScreenImage.screen-720p.jpg");
                    break;
            }
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
                switch (XResolutionHelper.CurrentResolution)
                {
                    case Resolutions.HD720p:
                        splash.SetSplashImage("../SplashScreenImage.screen-720p.jpg");
                        break;
                    case Resolutions.WVGA:
                        splash.SetSplashImage("../SplashScreenImage.screen-WVGA.jpg");
                        break;
                    case Resolutions.WXGA:
                        splash.SetSplashImage("../SplashScreenImage.screen-WXGA.jpg");
                        break;
                    default:
                        splash.SetSplashImage("../SplashScreenImage.screen-720p.jpg");
                        break;
                }
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
    }
}
