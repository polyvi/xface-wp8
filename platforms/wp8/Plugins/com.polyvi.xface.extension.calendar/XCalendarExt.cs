using System;
using Microsoft.Phone.Tasks;

namespace WPCordovaClassLib.Cordova.Commands
{
    class Calendar : BaseCommand
    {

        /// <summary>
        /// Used to open datetime picker
        /// </summary>
        private XDateTimePickerTask dateTimePickerTask;


        public void getTime(string options)
        {
            this.dateTimePickerTask = new XDateTimePickerTask();
            dateTimePickerTask.Value = System.DateTime.Now;

            dateTimePickerTask.Completed += this.DateTimePickerTask_Completed;
            dateTimePickerTask.Show(XDateTimePickerTask.DateTimePickerType.Time);

        }

        public void getDate(string options)
        {
            this.dateTimePickerTask = new XDateTimePickerTask();
            dateTimePickerTask.Value = System.DateTime.Now;

            dateTimePickerTask.Completed += this.DateTimePickerTask_Completed;
            dateTimePickerTask.Show(XDateTimePickerTask.DateTimePickerType.Date);
        }

        /// <summary>
        /// Handles datetime picker result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">stores information about picker dateTime</param>
        private void DateTimePickerTask_Completed(object sender, XDateTimePickerTask.DateTimeResult e)
        {

            if (e.Error != null)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                return;
            }

            switch (e.TaskResult)
            {
                case TaskResult.OK:
                    try
                    {
                        string res = String.Format("\"year\":{0},\"month\":{1},\"day\":{2},\"hour\":{3},\"minute\":{4}",
                                        e.Value.Value.Year,
                                        e.Value.Value.Month,
                                        e.Value.Value.Day,
                                        e.Value.Value.Hour,
                                        e.Value.Value.Minute);
                        res = "{" + res + "}";
                        DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res));
                    }
                    catch (ArgumentException)
                    {
                        DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
                    }
                    break;

                case TaskResult.Cancel:
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Canceled."));
                    break;
            }

            this.dateTimePickerTask = null;
        }
    }
}
