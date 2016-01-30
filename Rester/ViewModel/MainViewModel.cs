using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Rester.Model;

namespace Rester.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly IServiceStore _serviceStore;
        private readonly INavigationService _navigationService;

        public MainViewModel(IServiceStore serviceStore, INavigationService navigationService)
        {
            _serviceStore = serviceStore;
            _navigationService = navigationService; //For some reason, the navigation does not work if not kept as a member
            LoadData();
            NavigateToLogCommand = new RelayCommand(() => _navigationService.NavigateTo(LogPage.Key));
            EditModeCommand = new RelayCommand(() => EditMode = true);
            EditCompletedCommand = new RelayCommand(() => EditMode = false); //TODO: Save to store now or on every change?
        }

        private async void LoadData()
        {
            var configs = await _serviceStore.GetServiceConfigurations();
            ServiceConfigurations.ClearAndAddRange(configs);
        }

        public ObservableCollection2<ServiceConfiguration> ServiceConfigurations { get; } = new ObservableCollection2<ServiceConfiguration>();

        public bool EditMode { get { return _editMode; } set { Set(nameof(EditMode), ref _editMode, value); } }
        private bool _editMode;

        public ICommand NavigateToLogCommand { get; }
        public ICommand EditModeCommand { get; }
        public ICommand EditCompletedCommand { get; }
    }
}