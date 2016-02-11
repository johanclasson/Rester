using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Rester.Service
{
    internal interface IDialog
    {
        Task<string> ShowAsync(string message, string title, string defaultCommad = null, string cancelCommand = null);
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class Dialog : IDialog
    {
        // Windows 10 Mobile throws Argument Exception if more than two commands are used
        public async Task<string> ShowAsync(string message, string title, string defaultCommad = null, string cancelCommand = null)
        {
            if (defaultCommad == null)
                defaultCommad = "Ok";
            var dialog = new MessageDialog(message, title)
            {
                DefaultCommandIndex = 0
            };
            dialog.Commands.Add(new UICommand(defaultCommad));
            if (cancelCommand != null)
            {
                dialog.Commands.Add(new UICommand(cancelCommand));
                dialog.CancelCommandIndex = 1;
            }
            var result = await dialog.ShowAsync();
            return result.Label;
        }
    }
}
