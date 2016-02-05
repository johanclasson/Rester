using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Rester.Model;
using Rester.Service;

namespace Rester.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly IServiceStore _serviceStore;
        private readonly INavigationService _navigationService;
        private readonly IActionInvokerFactory _invokerFactory;

        public MainViewModel(IServiceStore serviceStore, INavigationService navigationService, IActionInvokerFactory invokerFactory)
        {
            _serviceStore = serviceStore;
            _navigationService = navigationService; //For some reason, the navigation does not work if not kept as a member
            _invokerFactory = invokerFactory;
            LoadDataAsync();
            InitHandlers();
            Messenger.Default.Register<SomethingIsChangedMessage>(this, async _ => await StoreDataAsync());
            NavigateToLogCommand = new RelayCommand(() => _navigationService.NavigateTo(LogPage.Key));
            EditModeCommand = new RelayCommand(() => EditMode = true);
            EditCompletedCommand = new RelayCommand(() => EditMode = false); //TODO: Save to store now or on every change?
            DeleteConfigurationCommand = new RelayCommand<ServiceConfiguration>(async configuration =>
            {
                ServiceConfigurations.Remove(configuration);
                await StoreDataAsync();
            });
            AddConfigurationCommand = new RelayCommand(AddEmptyServiceConfiguration);
        }

        private async void AddEmptyServiceConfiguration()
        {
            ServiceConfigurations.Add(ServiceConfiguration.CreateSilently("", "", _navigationService, _invokerFactory, EditMode));
            await StoreDataAsync();
        }

        private Task StoreDataAsync()
        {
            return _serviceStore.SaveServiceConfigurations(ServiceConfigurations);
        }

        void InitHandlers()
        {
            ApplicationData.Current.DataChanged += (_, __) =>
            {
                //TODO: Is this method called also when the current app saves data to the roaming store?
                LoadDataAsync();
            };
        }

        private async void LoadDataAsync()
        {
            var configs = await _serviceStore.LoadServiceConfigurations();
            ServiceConfigurations.ClearAndAddRange(configs);
        }

        public ObservableCollectionWithAddRange<ServiceConfiguration> ServiceConfigurations { get; } = new ObservableCollectionWithAddRange<ServiceConfiguration>();

        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            private set
            {
                Set(nameof(EditMode), ref _editMode, value);
                Messenger.Default.Send(new EditModeChangedMessage(value));
            }
        }

        private bool _editMode;

        public ICommand NavigateToLogCommand { get; }
        public ICommand EditModeCommand { get; }
        public ICommand EditCompletedCommand { get; }
        public ICommand DeleteConfigurationCommand { get; }
        public ICommand AddConfigurationCommand { get; }
    }
}