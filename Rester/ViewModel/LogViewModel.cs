using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Rester.Model;
using Rester.Service;

namespace Rester.ViewModel
{
    internal class LogViewModel : ViewModelBase
    {
        private readonly ILogStore _logStore;

        public LogViewModel(ILogStore logStore)
        {
            _logStore = logStore;
            Messenger.Default.Register<NotificationMessage<HttpResponse>>(this,
                message => LogEntries.Insert(0, message.Content));
            LoadOnlyForToday();
            LoadData();
        }

        private async void LoadOnlyForToday()
        {
            _showOnlyFromToday = await _logStore.GetOnlyFromToday();
        }

        private async void LoadData()
        {
            var entries = await _logStore.GetLogEntries();
            LogEntries.ClearAndAddRange(entries);
        }

        public ObservableCollectionWithAddRange<HttpResponse> LogEntries { get; } = new ObservableCollectionWithAddRange<HttpResponse>();

        public bool ShowOnlyFromToday
        {
            get
            {
                return _showOnlyFromToday;
            }
            set
            {
                Set(nameof(ShowOnlyFromToday), ref _showOnlyFromToday, value);
                ReloadData();
            }
        }
        private bool _showOnlyFromToday;

        private async void ReloadData()
        {
            await _logStore.SetOnlyFromToday(ShowOnlyFromToday);
            LoadData();
        }
    }
}