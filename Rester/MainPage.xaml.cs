using GalaSoft.MvvmLight.Messaging;
using Rester.Model;

namespace Rester
{
    public sealed partial class MainPage
    {
        private int _nrOfProcessingActions;

        public MainPage()
        {
            InitializeComponent();
            Messenger.Default.Register<ActionProcessingMessage>(this, _ =>
            {
                if (_nrOfProcessingActions++ == 0)
                    SpinningProgress.Begin();
            });
            Messenger.Default.Register<ActionCompletedMessage>(this, _ =>
            {
                _nrOfProcessingActions--;
                if (_nrOfProcessingActions == 0)
                    SpinningProgress.Stop();
            });
        }
    }
}
