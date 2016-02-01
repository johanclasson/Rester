using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Rester.Service;

namespace Rester.Model
{
    internal class ServiceEndpoint : AbstractResterModel
    {
        public ServiceEndpoint(bool editMode = false) : base(editMode)
        {
            AddActionCommand = new RelayCommand(() =>
            {
                Actions.Add(new ServiceEndpointAction(EditMode));
                //TODO: Navigate to action page
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