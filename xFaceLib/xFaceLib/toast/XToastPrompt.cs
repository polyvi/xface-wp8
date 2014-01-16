using System.Windows;
using Coding4Fun.Toolkit.Controls;

namespace xFaceLib.toast
{
    class XToastPrompt
    {
        private static XToastPrompt instance;

        public static XToastPrompt GetInstance()
        {
            if (null == instance)
            {
                instance = new XToastPrompt();
            }
            return instance;
        }

        public void Toast(string content, string title = "", int timeout = 3000)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToastPrompt toast = new ToastPrompt();
                toast.Title = title;
                toast.Message = content;
                toast.MillisecondsUntilHidden = timeout;
                toast.TextWrapping = TextWrapping.Wrap;
                toast.Show();
            });
        }
    }
}
