using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace xFaceLib.extensions.zbar
{
    public class XBarCodeTask
    {
        public class BarCodeResult : TaskEventArgs
        {
            /// <summary>
            /// Initializes a new instance of the BarCodeResult class.
            /// </summary>
            public BarCodeResult()
            { }

            /// <summary>
            /// Initializes a new instance of the BarCodeResult class
            /// with the specified Microsoft.Phone.Tasks.TaskResult.
            /// </summary>
            /// <param name="taskResult">Associated Microsoft.Phone.Tasks.TaskResult</param>
            public BarCodeResult(TaskResult taskResult)
                : base(taskResult)
            { }

            /// <summary>
            ///  Gets the result string of the barcode.
            /// </summary>
            public string result { get; internal set; }
        }
        /// <summary>
        /// Occurs when a BarCode task is completed.
        /// </summary>
        public event EventHandler<BarCodeResult> Completed;

        /// <summary>
        /// Shows BarcodeScanner application
        /// </summary>
        public void Show()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var root = Application.Current.RootVisual as PhoneApplicationFrame;

                root.Navigated += new System.Windows.Navigation.NavigatedEventHandler(NavigationService_Navigated);

                string baseUrl = "/";
                // 使用type参数qrcode 默认解析二维码
                root.Navigate(new System.Uri(baseUrl + "Plugins/com.polyvi.xface.extension.zbar/XBarCode.xaml?type=qrcode", UriKind.Relative));
            });
        }

        /// <summary>
        /// Performs additional configuration of the barcode application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!(e.Content is XBarCode)) return;

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigated -= NavigationService_Navigated;

            XBarCode barCode = (XBarCode)e.Content;

            if (barCode != null)
            {
                barCode.Completed += this.Completed;
            }
            else if (this.Completed != null)
            {
                this.Completed(this, new BarCodeResult(TaskResult.Cancel));
            }
        }
    }
}
