using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Callisto.TestApp
{
    public static class TaskExtender
    {
        public static Task ChainFailureHandler(this Task task, Page page)
        {
            // create a failure handler...
            var handler = task.ContinueWith((t, args) =>
            {
                // show the error...
                if (task.Exception != null)
                    page.ShowAlertAsync(task.Exception.ToString());
                else
                    page.ShowAlertAsync("An error occurred but no error was received.");

            }, TaskContinuationOptions.OnlyOnFaulted);

            // return...
            return handler;
        }
    }
}
