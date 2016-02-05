using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Rester.Service;

namespace Rester.Model
{
    internal class ActionProcessingMessage { }
    internal class ActionCompletedMessage { }

    public class ServiceConfiguration : AbstractResterModel
    {
        private readonly INavigationService _navigationService;
        private readonly IActionInvokerFactory _invokerFactory;

        public static ServiceConfiguration CreateSilently(string name, string baseUri, INavigationService navigationService,
            IActionInvokerFactory invokerFactory, bool editMode = false)
        {
            return new ServiceConfiguration(navigationService, invokerFactory, editMode)
            {
                _name = name,
                _baseUri = baseUri
            };
        }

        private ServiceConfiguration(INavigationService navigationService, IActionInvokerFactory invokerFactory, bool editMode) : base(editMode)
        {
            _navigationService = navigationService;
            _invokerFactory = invokerFactory;
            Endpoints = new ObservableCollectionWithAddRange<ServiceEndpoint>();
            AddEndpointCommand = new RelayCommand(() =>
            {
                Endpoints.Add(ServiceEndpoint.CreateSilently("", this, _navigationService, EditMode));
                NotifyThatSomethingIsChanged();
            });
            InvokeUriCommand = new RelayCommand<ServiceEndpointAction>(async action =>
            {
                if (EditMode)
                {
                    _navigationService.NavigateTo(ActionPage.Key, action);
                }
                else
                {
                    action.Processing = true;
                    Messenger.Default.Send(new ActionProcessingMessage());
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
                    await Task.Delay(1000); //The spinner animation should run at least 1 sekond
                    Messenger.Default.Send(new ActionCompletedMessage());
                }
            }, action => action != null && !action.Processing);
            DeleteEndpointCommand = new RelayCommand<ServiceEndpoint>(endpoint =>
            {
                Endpoints.Remove(endpoint);
                NotifyThatSomethingIsChanged();
            });
        }

        public string BaseUri { get { return _baseUri; } set { SetAndSave(nameof(BaseUri), ref _baseUri, value); } }
        private string _baseUri;

        public string Name { get { return _name; } set { SetAndSave(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollectionWithAddRange<ServiceEndpoint> Endpoints { get; }
        public ICommand AddEndpointCommand { get; }

        public ICommand InvokeUriCommand { get; }
        public ICommand DeleteEndpointCommand { get; }
    }
}
