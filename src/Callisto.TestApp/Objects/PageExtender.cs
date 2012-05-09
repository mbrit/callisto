using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Callisto.TestApp
{
    public static class PageExtender
    {
        internal static IAsyncOperation<IUICommand> ShowAlertAsync(this Page page, Exception ex)
        {
            return ShowAlertAsync(page,ex.ToString());
        }

        internal static IAsyncOperation<IUICommand> ShowAlertAsync(this Page page, string message)
        {
            // do we need to flip threads?
            if (!(page.Dispatcher.HasThreadAccess))
            {
                IAsyncOperation<IUICommand> result = null;
                page.Dispatcher.Invoke(Windows.UI.Core.CoreDispatcherPriority.Normal, (sender, e) =>
                {
                    result = ShowAlertAsync(page, message);
                }, page, null);

                // return...
                return result;
            }

            // show...
            MessageDialog dialog = new MessageDialog(message != null ? message : string.Empty);
            return dialog.ShowAsync();
        }
    }
}
