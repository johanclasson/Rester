using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Rester.Service;

namespace Rester.Model
{
    internal class ServiceEndpoint : AbstractResterModel
    {
        private readonly ServiceConfiguration _configuration;
        private readonly INavigationService _navigationService;

        public ServiceEndpoint(ServiceConfiguration configuration, INavigationService navigationService, bool editMode = false) : base(editMode)
        {
            _configuration = configuration;
            _navigationService = navigationService;
            AddActionCommand = new RelayCommand(() =>
            {
                var action = new ServiceEndpointAction(() => _configuration.BaseUri, EditMode)
                {
                    MediaType = "application/json",
                    Method = "Get"
                };
                Actions.Add(action);
                _navigationService.NavigateTo(ActionPage.Key, action);
            });
            DeleteActionCommand = new RelayCommand<ServiceEndpointAction>(action => Actions.Remove(action));
        }

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollectionWithAddRange<ServiceEndpointAction> Actions { get; } = new ObservableCollectionWithAddRange<ServiceEndpointAction>();

        public ICommand AddActionCommand { get; }
        public ICommand DeleteActionCommand { get; }
    }
}