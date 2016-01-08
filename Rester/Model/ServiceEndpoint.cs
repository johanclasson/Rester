using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;

namespace Rester.Model
{
    public class ServiceEndpoint : ObservableObject
    {
        public ServiceEndpoint()
        {
            Actions = new ObservableCollection<ServiceEndpointAction>();
        }

        public ServiceEndpoint(IEnumerable<ServiceEndpointAction> actions)
        {
            Actions = new ObservableCollection<ServiceEndpointAction>(actions);
        }

        public Symbol Symbol { get { return _symbol; } set { Set(nameof(Symbol), ref _symbol, value); } }
        private Symbol _symbol;

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollection<ServiceEndpointAction> Actions { get; }

    }
}