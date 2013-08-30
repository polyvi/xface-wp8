using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace xFaceLib.SplashScreenControl
{
    public partial class XSplashScreenControl : UserControl
    {
        public XSplashScreenControl()
        {
            InitializeComponent();

            this.progressBar1.IsIndeterminate = true;
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
        }
    }
}
