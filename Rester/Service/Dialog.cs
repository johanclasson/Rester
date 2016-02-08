using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Rester.Service
{
    internal interface IDialog
    {
        Task<string> ShowAsync(string message, string title, string[] buttonLabels = null, uint cancelCommandIndex = 99999, uint defaultCommandIndex = 0);
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class Dialog : IDialog
    {
        public async Task<string> ShowAsync(string message, string title, string[] buttonLabels = null, uint cancelCommandIndex = 99999, uint defaultCommandIndex = 0)
        {
            if (buttonLabels == null || buttonLabels.Length == 0)
                buttonLabels = new[] {"Ok"};
            if (cancelCommandIndex == 99999)
                cancelCommandIndex = (uint)buttonLabels.Length - 1;
            var dialog = new MessageDialog(message, title)
            {
                CancelCommandIndex = cancelCommandIndex,
                DefaultCommandIndex = defaultCommandIndex
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
