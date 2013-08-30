using System;
using Microsoft.Phone.Notification;
using System.Text;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Linq;
using xFaceLib.Log;
using xFaceLib.Util;

namespace xFaceLib.runtime
{
    public class XPushNotificationHelper
    {
        public static event EventHandler<string> PushNotificationEventHandler;

        public XPushNotificationHelper()
        {
            /// Holds the push channel that is created or found.
            HttpNotificationChannel pushChannel;

            // The name of our push channel.
            string channelName = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("ProductID").Value;

            // Try to find the push channel.
            pushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);
                pushChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(PushChannel_HttpNotificationReceived);
                pushChannel.Open();

                //Raw push only work on app running

                // Bind this new channel for toast events.
                //Toast push is work app is running or background!
                pushChannel.BindToShellToast();
                // Bind this new channel for tile events.
                //Tile push is work background or died!
                pushChannel.BindToShellTile();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);
                pushChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(PushChannel_HttpNotificationReceived);

                if (null == pushChannel.ChannelUri)
                    return;

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                XLog.WriteInfo("push channel URI is : " + pushChannel.ChannelUri.ToString());
                //保存 Uri，由push扩展获取发送到js端
                IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
                userSettings[XConstant.PUSH_NOTIFICATION_URI] = pushChannel.ChannelUri.ToString();
                userSettings.Save();
            }
        }

        /// <summary>
        /// Event handler for when the push channel Uri is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
            XLog.WriteInfo("push channel URI updated : " + e.ChannelUri.ToString());

            //保存 Uri，由push扩展获取发送到js端
            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
            userSettings[XConstant.PUSH_NOTIFICATION_URI] = e.ChannelUri.ToString();
            userSettings.Save();
        }

        /// <summary>
        /// Event handler for when a push notification error occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
            XLog.WriteError(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData));
        }

        /// <summary>
        /// Event handler for when a toast notification arrives while your application is running.  
        /// The toast will not display if your application is running so you must add this
        /// event handler if you want to do something with the toast notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);
            }
            XLog.WriteInfo("Toast notification :" + message.ToString());

            if (PushNotificationEventHandler != null)
            {
                PushNotificationEventHandler(null, message.ToString());
            }
        }

        /// <summary>
        /// Event handler for when a raw notification arrives while your application is running. 
        /// The raw will not display if your application is running so you must add this
        /// event handler if you want to do something with the raw notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            string message;

            using (System.IO.StreamReader reader = new System.IO.StreamReader(e.Notification.Body))
            {
                message = reader.ReadToEnd();
            }

            XLog.WriteInfo("Raw notification :" + message);

            if (PushNotificationEventHandler != null)
            {
                PushNotificationEventHandler(null, message.ToString());
            }
        }
    }
}
