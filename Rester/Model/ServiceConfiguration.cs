using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Rester.Service;

namespace Rester.Model
{
    internal class ServiceConfiguration : AbstractResterModel
    {
        public ServiceConfiguration()
        {
            Endpoints = new ObservableCollectionWithAddRange<ServiceEndpoint>();
            AddEndpointCommand = new RelayCommand(() => { Endpoints.Add(new ServiceEndpoint(EditMode)); });
        }

        public string BaseUri { get { return _baseUri; } set { Set(nameof(BaseUri), ref _baseUri, value); } }
        private string _baseUri;

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollectionWithAddRange<ServiceEndpoint> Endpoints { get; }
        public ICommand AddEndpointCommand { get; }
    }
}
