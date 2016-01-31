using System;
using System.Windows.Input;
using Windows.UI.Popups;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Rester.Service;

namespace Rester.Model
{
    internal class ServiceEndpointAction : AbstractResterModel
    {
        private readonly IHttpClient _httpClient;
        private ServiceConfiguration Configuration { get; }

        public ServiceEndpointAction(ServiceConfiguration configuration, IHttpClient httpClient)
        {
            _httpClient = httpClient;
            Configuration = configuration;
            InvokeUriCommand = new RelayCommand(async () =>
            {
                Processing = true;
                ((RelayCommand)InvokeUriCommand).RaiseCanExecuteChanged();
                try
                {
                    var response = await _httpClient.InvokeRestAction(this);
                    Messenger.Default.Send(new NotificationMessage<HttpResponse>(response, "Service Endpoint Action Result"));
                }
                catch (Exception ex)
                {
                    await new MessageDialog($"Something bad happended: {ex.Message}").ShowAsync(); //TODO: Move dialog call to somewhere else
                }
                Processing = false;
                ((RelayCommand)InvokeUriCommand).RaiseCanExecuteChanged();
            }, () => !Processing);
        }

        public Uri Uri => new Uri(new Uri(Configuration.BaseUri), UriPath);

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

        private bool Processing { get; set; }

        public ICommand InvokeUriCommand { get; }
    }
}