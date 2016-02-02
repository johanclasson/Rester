using System.Windows.Input;
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
            LoadData();
            NavigateToLogCommand = new RelayCommand(() => _navigationService.NavigateTo(LogPage.Key));
            EditModeCommand = new RelayCommand(() => EditMode = true);
            EditCompletedCommand = new RelayCommand(() => EditMode = false); //TODO: Save to store now or on every change?
            DeleteConfigurationCommand = new RelayCommand<ServiceConfiguration>(configuration => ServiceConfigurations.Remove(configuration));
            AddConfigurationCommand = new RelayCommand(() => ServiceConfigurations.Add(new ServiceConfiguration(_navigationService, _invokerFactory, EditMode)));
        }

        private async void LoadData()
        {
            var configs = await _serviceStore.GetServiceConfigurations();
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