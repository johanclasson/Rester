using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Rester.Service;

namespace Rester.Model
{
    internal class ServiceEndpoint : AbstractResterModel
    {
        private readonly INavigationService _navigationService;

        public ServiceEndpoint(INavigationService navigationService, bool editMode = false) : base(editMode)
        {
            _navigationService = navigationService;
            AddActionCommand = new RelayCommand(() =>
            {
                var action = new ServiceEndpointAction(EditMode)
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