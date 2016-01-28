using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Rester.Model
{
    internal class ServiceEndpointAction : ObservableObject
    {
        private ServiceConfiguration Configuration { get; }

        public ServiceEndpointAction(ServiceConfiguration configuration)
        {
            Configuration = configuration;
            InvokeUriCommand = new RelayCommand(async () =>
            {
                Processing = true;
                ((RelayCommand)InvokeUriCommand).RaiseCanExecuteChanged();
                try
                {
                    HttpResponseMessage result = await InvokeUriAsync();
                    if (result.StatusCode != HttpStatusCode.OK)
                        await new MessageDialog($"Resultat: {result.StatusCode}").ShowAsync();
                }
                catch (Exception ex)
                {
                    await new MessageDialog($"Something bad happended: {ex.Message}").ShowAsync();
                }
                Processing = false;
                ((RelayCommand)InvokeUriCommand).RaiseCanExecuteChanged();
            }, () => !Processing);
        }

        private Uri Uri => new Uri(new Uri(Configuration.BaseUri), UriPath);

        private async Task<HttpResponseMessage> InvokeUriAsync()
        {
            using (var client = new HttpClient())
            {
                switch (Method.ToLower())
                {
                    case "get":
                        return await client.GetAsync(Uri);
                    case "put":
                        return await client.PutAsync(Uri, new StringContent(Body, Encoding.UTF8, MediaType));
                    case "post":
                        return await client.PostAsync(Uri, new StringContent(Body, Encoding.UTF8, MediaType));
                    case "delete":
                        return await client.DeleteAsync(Uri);
                    default:
                        throw new ArgumentException($"Encountered unknown http method {Method}");
                }
            }
        }

        public string UriPath { get { return _uriPath; } set { Set(nameof(UriPath), ref _uriPath, value); } }
        private string _uriPath;

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public string Method { get { return _method; } set { Set(nameof(Method), ref _method, value); } }
        private string _method;

        public string Body { get { return _body; } set { Set(nameof(Body), ref _body, value); } }
        private string _body;

        public string MediaType { get { return _mediaType; } set { Set(nameof(MediaType), ref _mediaType, value); } }
        private string _mediaType;

        public bool Processing { get { return _processing; } private set { Set(nameof(Processing), ref _processing, value); } }
        private bool _processing;

        public ICommand InvokeUriCommand { get; }
    }
}