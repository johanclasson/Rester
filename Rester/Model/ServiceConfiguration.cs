using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Rester.Model
{
    public class ServiceConfiguration : ObservableObject
    {
        public ServiceConfiguration()
        {
            Endpoints = new ObservableCollection<ServiceEndpoint>();
        }

        public ServiceConfiguration(IEnumerable<ServiceEndpoint> endpoints)
        {
            Endpoints = new ObservableCollection<ServiceEndpoint>(endpoints);
        }

        public string UriRoot { get { return _uriRoot; } set { Set(nameof(UriRoot), ref _uriRoot, value); } }
        private string _uriRoot;

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollection<ServiceEndpoint> Endpoints { get; }
    }
}
