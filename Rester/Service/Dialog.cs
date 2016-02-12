using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Rester.Service
{
    internal interface IDialog
    {
        //It is possible to use up to two commands on a phone before MessageDialog throws exception
        Task<string> ShowAsync(string message, string title, string defaultCommad = null, string cancelCommand = null);
#if X86X64
        //It is possible to use up to three commands in a desktop application before MessageDialog throws exception
        Task<string> ShowAsync(string message, string title, string defaultCommad, string otherCommand, string cancelCommand);
#endif
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class Dialog : IDialog
    {
        // Windows 10 Mobile throws Argument Exception if more than two commands are used
        public Task<string> ShowAsync(string message, string title, string defaultCommad = null, string cancelCommand = null)
        {
            var buttonLabels = new List<string>();
            if (defaultCommad != null)
                buttonLabels.Add(defaultCommad);
            if (cancelCommand != null)
                buttonLabels.Add(cancelCommand);
            return ShowAsync(message, title, buttonLabels.ToArray());
        }

        public Task<string> ShowAsync(string message, string title, string defaultCommad, string otherCommand, string cancelCommand)
        {
            return ShowAsync(message, title, new[] {defaultCommad, otherCommand, cancelCommand});
        }

        private async Task<string> ShowAsync(string message, string title, string[] buttonLabels = null)
        {
            if (buttonLabels == null || buttonLabels.Length == 0)
                buttonLabels = new[] { "Ok" };
            var dialog = new MessageDialog(message, title)
            {
                CancelCommandIndex = (uint)buttonLabels.Length - 1,
                DefaultCommandIndex = 0
            };
            foreach (string label in buttonLabels)
            {
                dialog.Commands.Add(new UICommand(label));
            }
            var result = await dialog.ShowAsync();
            return result.Label;
        }
    }
}
