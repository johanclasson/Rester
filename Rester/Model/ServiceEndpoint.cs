using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Rester.Service;

namespace Rester.Model
{
    public class ServiceEndpoint : AbstractResterModel
    {
        private readonly ServiceConfiguration _configuration;
        private readonly INavigationService _navigationService;

        public static ServiceEndpoint CreateSilently(string name, ServiceConfiguration configuration,
            INavigationService navigationService, bool editMode = false)
        {
            return new ServiceEndpoint(configuration, navigationService, editMode)
            {
                _name = name
            };
        }

        private ServiceEndpoint(ServiceConfiguration configuration, INavigationService navigationService, bool editMode) : base(editMode)
        {
            _configuration = configuration;
            _navigationService = navigationService;
            AddActionCommand = new RelayCommand(() =>
            {
                var action = ServiceEndpointAction.CreateSilently("", "", "Get", "", "application/json",
                    () => _configuration.BaseUri, EditMode);
                Actions.Add(action);
                NotifyThatSomethingIsChanged();
                _navigationService.NavigateTo(ActionPage.Key, action);
            });
            DeleteActionCommand = new RelayCommand<ServiceEndpointAction>(action =>
            {
                Actions.Remove(action);
                NotifyThatSomethingIsChanged();
            });
        }

        public string Name { get { return _name; } set { SetAndSave(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollectionWithAddRange<ServiceEndpointAction> Actions { get; } = new ObservableCollectionWithAddRange<ServiceEndpointAction>();

        public ICommand AddActionCommand { get; }
        // ReSharper disable once MemberCanBePrivate.Global - It is used by a child binding through element name
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICommand DeleteActionCommand { get; }
    }
}