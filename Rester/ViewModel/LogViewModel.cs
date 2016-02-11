using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Rester.Model;
using Rester.Service;

namespace Rester.ViewModel
{
    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class LogViewModel : ViewModelBase
    {
        private readonly ILogStore _logStore;

        public LogViewModel(ILogStore logStore)
        {
            _logStore = logStore;
            Messenger.Default.Register<NotificationMessage<HttpResponse>>(this, async message=> await AddLogEntry(message.Content));
            LoadOnlyForToday();
            LoadData();
        }

        private async Task AddLogEntry(HttpResponse logEntry)
        {
            LogEntries.Insert(0, logEntry);
            await _logStore.AddAsync(logEntry);
        }

        private async void LoadOnlyForToday()
        {
            _showOnlyFromToday = await _logStore.GetOnlyFromTodayAsync();
        }

        private async void LoadData()
        {
            var entries = await _logStore.GetLogEntriesAsync();
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
            await _logStore.SetOnlyFromTodayAsync(ShowOnlyFromToday);
            LoadData();
        }
    }
}