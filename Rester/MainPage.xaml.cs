using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using Rester.Model;

namespace Rester
{
    public sealed partial class MainPage : Page
    {
        private int _nrOfProcessingActions = 0;

        public MainPage()
        {
            this.InitializeComponent();
            Messenger.Default.Register<ActionProcessingMessage>(this, (_) =>
            {
                if (_nrOfProcessingActions++ == 0)
                    SpinningProgress.Begin();
            });
            Messenger.Default.Register<ActionCompletedMessage>(this, (_) =>
            {
                _nrOfProcessingActions--;
                if (_nrOfProcessingActions == 0)
                    SpinningProgress.Stop();
            });
        }
    }
}
