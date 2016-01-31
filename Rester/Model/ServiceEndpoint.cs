using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rester.Model
{
    internal class ServiceEndpoint : AbstractResterModel
    {
        public ServiceEndpoint()
        {
            Actions = new ObservableCollection<ServiceEndpointAction>();
        }

        public ServiceEndpoint(IEnumerable<ServiceEndpointAction> actions)
        {
            Actions = new ObservableCollection<ServiceEndpointAction>(actions);
        }

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollection<ServiceEndpointAction> Actions { get; }

    }
}