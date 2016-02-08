using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Rester.Model;
using Rester.ViewModel;

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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var storageFiles = e.Parameter as StorageFile[];
            if (storageFiles == null)
                return;
            var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
            foreach (StorageFile storageFile in storageFiles)
            {
                await mainViewModel.ImportConfigurationsFromFileAsync(storageFile);
            }
        }
    }
}
