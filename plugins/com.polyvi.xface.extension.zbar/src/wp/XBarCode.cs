using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using com.google.zxing;
using com.google.zxing.oned;
using com.google.zxing.qrcode;
using com.google.zxing.common;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using BarCodeResult = xFaceLib.extensions.zbar.XBarCodeTask.BarCodeResult;

namespace xFaceLib.extensions.zbar
{
    public partial class XBarCode : PhoneApplicationPage
    {
        private PhotoCamera _photoCamera;
        private XPhotoCameraLuminanceSource _luminance;
        private readonly DispatcherTimer _timer;
        private Reader _reader = null;

        private bool isInitialized = false;
        /// <summary>
        /// Occurs when a BarCode task is completed.
        /// </summary>
        public event EventHandler<BarCodeResult> Completed;

        /// <summary>
        /// BarCode result, dispatched back when BarCode page is closed
        /// </summary>
        private BarCodeResult result = new BarCodeResult(TaskResult.Cancel);

        public XBarCode()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += (o, arg) => ScanPreviewBuffer();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                this.IsEnabled = false;
                base.NavigationService.GoBack();
            }
            else
            {
                isInitialized = false;
                string type = "";
                if (NavigationContext.QueryString.TryGetValue("type", out type) && type == "qrcode")
                {
                    _reader = new QRCodeReader();
                }
                else
                {
                    _reader = new EAN13Reader();
                }

                _photoCamera = new PhotoCamera();
                _photoCamera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
                _videoBrush.SetSource(_photoCamera);
                BarCodeRectInitial();
                base.OnNavigatedTo(e);
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            //切到外部程序不处理该事件。如按start/search key的情况
            if (e.Uri.OriginalString.Equals("app://external/"))
                return;

            if (_photoCamera != null)
            {
                _timer.Stop();
                _photoCamera.Dispose();
                _photoCamera.Initialized -= cam_Initialized;
            }
            if (Completed != null)
            {
                Completed(this, result);
            }
            base.OnNavigatingFrom(e);
        }

        //FIXME: 快速启动相机点击 back/start/search key will occur photocamera exception
        //此处只能出来backkey事件，没法处理 start/search 事件
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (isInitialized == false)
            {
                // If the Camera is not initialised, cancel the back key press.
                e.Cancel = true;
            }
        }

        void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                isInitialized = true;
                int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
                int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);
                _luminance = new XPhotoCameraLuminanceSource(width, height);

                Dispatcher.BeginInvoke(() =>
                {
                    _previewTransform.Rotation = _photoCamera.Orientation;
                    _timer.Start();
                });
                _photoCamera.FlashMode = FlashMode.Auto;
                _photoCamera.Focus();
            }
        }

        public void SetStillPicture()
        {
            int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
            int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);
            int[] PreviewBuffer = new int[width * height];
            _photoCamera.GetPreviewBufferArgb32(PreviewBuffer);

            WriteableBitmap wb = new WriteableBitmap(width, height);
            PreviewBuffer.CopyTo(wb.Pixels, 0);

            MemoryStream ms = new MemoryStream();
            wb.SaveJpeg(ms, wb.PixelWidth, wb.PixelHeight, 0, 80);
            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage bi = new BitmapImage();
            bi.SetSource(ms);
            ImageBrush still = new ImageBrush();
            still.ImageSource = bi;
            frame.Fill = still;
            still.RelativeTransform = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = _photoCamera.Orientation };
        }

        private void ScanPreviewBuffer()
        {
            try
            {
                _photoCamera.GetPreviewBufferY(_luminance.PreviewBufferY);
                var binarizer = new HybridBinarizer(_luminance);
                var binBitmap = new BinaryBitmap(binarizer);
                Result result = _reader.decode(binBitmap);
                if (result != null)
                {
                    _timer.Stop();
                    SetStillPicture();
                    BarCodeRectSuccess();
                    BarCodeResult barCodeResult = new BarCodeResult(TaskResult.OK);
                    barCodeResult.result = result.Text;
                    Dispatcher.BeginInvoke(() =>
                    {
                        this.result = barCodeResult;
                        if (this.NavigationService.CanGoBack)
                        {
                            this.NavigationService.GoBack();
                        }
                    });
                }
                else
                {
                    _photoCamera.Focus();
                }
            }
            catch
            {
                //do nothing
            }

        }

        void BarCodeRectSuccess()
        {
            Dispatcher.BeginInvoke(() =>
            {
                _marker1.Fill = new SolidColorBrush(Colors.Green);
                _marker2.Fill = new SolidColorBrush(Colors.Green);
                _marker3.Fill = new SolidColorBrush(Colors.Green);
                _marker4.Fill = new SolidColorBrush(Colors.Green);
                _marker5.Fill = new SolidColorBrush(Colors.Green);
                _marker6.Fill = new SolidColorBrush(Colors.Green);
                _marker7.Fill = new SolidColorBrush(Colors.Green);
                _marker8.Fill = new SolidColorBrush(Colors.Green);
            });
        }

        void BarCodeRectInitial()
        {
            _marker1.Fill = new SolidColorBrush(Colors.Red);
            _marker2.Fill = new SolidColorBrush(Colors.Red);
            _marker3.Fill = new SolidColorBrush(Colors.Red);
            _marker4.Fill = new SolidColorBrush(Colors.Red);
            _marker5.Fill = new SolidColorBrush(Colors.Red);
            _marker6.Fill = new SolidColorBrush(Colors.Red);
            _marker7.Fill = new SolidColorBrush(Colors.Red);
            _marker8.Fill = new SolidColorBrush(Colors.Red);
        }
    }
}