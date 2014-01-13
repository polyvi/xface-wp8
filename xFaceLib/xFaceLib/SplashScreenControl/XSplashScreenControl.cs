using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using xFaceLib.Util;

namespace xFaceLib.SplashScreenControl
{
    public partial class XSplashScreenControl : UserControl
    {
        public XSplashScreenControl()
        {
            InitializeComponent();
            DetermineSplashImage();
            this.progressBar1.IsIndeterminate = true;
        }

        /// <summary>
        /// 设置是否显示进度条
        /// </summary>
        /// <param name="need"></param>
        public void SetNeedProgressBar(bool need)
        {
            this.progressBar1.IsIndeterminate = need;
        }

        /// <summary>
        /// 设置在splash上显示 xFace引擎的版本号和产品描述
        /// </summary>
        /// <param name="version">xFace引擎的版本号</param>
        /// <param name="product">产品描述</param>
        public void SetDisplayInfo(string version, string product)
        {
            this.Info.Text = version;
            this.Info.Inlines.Add(new LineBreak());
            var newLine = new Run();
            newLine.Text = product;
            this.Info.Inlines.Add(newLine);
        }

        /// <summary>
        /// 设置显示splash
        /// </summary>
        /// <param name="imagePath">指定的splash image 路径</param>
        public void SetSplashImage(string imagePath)
        {
            this.SplashImage.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            this.SplashImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.SplashImage.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            this.SplashImage.Width = System.Windows.Application.Current.Host.Content.ActualWidth;
            this.SplashImage.Height = System.Windows.Application.Current.Host.Content.ActualHeight;
        }

        /// <summary>
        /// 根据设备分辨率决定splashImage
        /// </summary>
        private void DetermineSplashImage()
        {
            switch (XResolutionHelper.CurrentResolution)
            {
                case Resolutions.HD1080p:
                    SetSplashImage("../SplashScreenImage-1080p.jpg");
                    break;
                case Resolutions.HD720p:
                    SetSplashImage("../SplashScreenImage-720p.jpg");
                    break;
                case Resolutions.WVGA:
                    SetSplashImage("../SplashScreenImage-WVGA.jpg");
                    break;
                case Resolutions.WXGA:
                    SetSplashImage("../SplashScreenImage-WXGA.jpg");
                    break;
                default:
                    SetSplashImage("../SplashScreenImage-1080p.jpg");
                    break;
            }
        }
    }
}
