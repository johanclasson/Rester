using GalaSoft.MvvmLight;

namespace Rester.Model
{
    public class ServiceEndpointAction : ObservableObject
    {
        public string Uri { get { return _uri; } set { Set(nameof(Uri), ref _uri, value); } }
        private string _uri;

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;
    }
}