using System;
using System.Windows.Input;
using Windows.UI.Popups;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Rester.Service;

namespace Rester.Model
{
    internal class ServiceConfiguration : AbstractResterModel
    {
        private readonly INavigationService _navigationService;
        private readonly IActionInvokerFactory _invokerFactory;

        public ServiceConfiguration(INavigationService navigationService, IActionInvokerFactory invokerFactory, bool editMode = false) : base(editMode)
        {
            _navigationService = navigationService;
            _invokerFactory = invokerFactory;
            Endpoints = new ObservableCollectionWithAddRange<ServiceEndpoint>();
            AddEndpointCommand = new RelayCommand(() => { Endpoints.Add(new ServiceEndpoint(this, _navigationService, EditMode)); });
            InvokeUriCommand = new RelayCommand<ServiceEndpointAction>(async action =>
            {
                if (EditMode)
                {
                    _navigationService.NavigateTo(ActionPage.Key, action);
                }
                else
                {
                    action.Processing = true;
                    ((RelayCommand<ServiceEndpointAction>) InvokeUriCommand).RaiseCanExecuteChanged();
                    try
                    {
                        var response = await _invokerFactory.CreateInvoker(BaseUri, action).InvokeRestAction();
                        Messenger.Default.Send(new NotificationMessage<HttpResponse>(response,
                            "Service Endpoint Action Result"));
                    }
                    catch (Exception ex)
                    {
                        await new MessageDialog($"Something bad happended: {ex.Message}").ShowAsync();
                            //TODO: Move dialog call to somewhere else
                    }
                    action.Processing = false;
                    ((RelayCommand<ServiceEndpointAction>) InvokeUriCommand).RaiseCanExecuteChanged();
                }
            }, action => action != null && !action.Processing);
            DeleteEndpointCommand = new RelayCommand<ServiceEndpoint>(endpoint => Endpoints.Remove(endpoint));
        }

        public string BaseUri { get { return _baseUri; } set { Set(nameof(BaseUri), ref _baseUri, value); } }
        private string _baseUri;

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollectionWithAddRange<ServiceEndpoint> Endpoints { get; }
        public ICommand AddEndpointCommand { get; }

        public ICommand InvokeUriCommand { get; }
        public ICommand DeleteEndpointCommand { get; }
    }
}
