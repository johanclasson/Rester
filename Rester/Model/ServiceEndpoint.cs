using Rester.Service;

namespace Rester.Model
{
    internal class ServiceEndpoint : AbstractResterModel
    {
        public ServiceEndpoint(bool editMode) : base(editMode)
        {
        }

        public ServiceEndpoint()
        {
        }

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollectionWithAddRange<ServiceEndpointAction> Actions { get; } = new ObservableCollectionWithAddRange<ServiceEndpointAction>();
    }
}