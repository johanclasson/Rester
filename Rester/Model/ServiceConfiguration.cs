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
            ActionGroups = new ObservableCollectionWithAddRange<ActionGroup>();
            AddActionGroupCommand = new RelayCommand(() =>
            {
                ActionGroups.Add(ActionGroup.CreateSilently("", this, _navigationService, EditMode));
                NotifyThatSomethingIsChanged();
            });
            InvokeUriCommand = new RelayCommand<ServiceAction>(async action =>
            {
                if (EditMode)
                {
                    _navigationService.NavigateTo(ActionPage.Key, action);
                }
                else
                {
                    action.Processing = true;
                    Messenger.Default.Send(new ActionProcessingMessage());
                    ((RelayCommand<ServiceAction>) InvokeUriCommand).RaiseCanExecuteChanged();
                    await InvokeRestActionAsync(action);
                    action.Processing = false;
                    ((RelayCommand<ServiceAction>) InvokeUriCommand).RaiseCanExecuteChanged();
                    await Task.Delay(1000); //The spinner animation should run at least 1 second
                    Messenger.Default.Send(new ActionCompletedMessage());
                }
            }, action => action != null && !action.Processing);
            DeleteActionGroupCommand = new RelayCommand<ActionGroup>(group =>
            {
                ActionGroups.Remove(group);
                NotifyThatSomethingIsChanged();
            });
        }

        private async Task InvokeRestActionAsync(ServiceAction action)
        {
            try
            {
                var response = await _invokerFactory.CreateInvoker(action).InvokeRestActionAsync();
                Messenger.Default.Send(new NotificationMessage<HttpResponse>(response,
                    "Service Action Result"));
            }
            catch (Exception ex)
            {
                //TODO: Move dialog call to somewhere else
                await new MessageDialog($"Something bad happended: {ex.Message}").ShowAsync();
            }
        }

        public string BaseUri { get { return _baseUri; } set { SetAndSave(nameof(BaseUri), ref _baseUri, value); } }
        private string _baseUri;

        public string Name { get { return _name; } set { SetAndSave(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollectionWithAddRange<ActionGroup> ActionGroups { get; }
        public ICommand AddActionGroupCommand { get; }

        // ReSharper disable once MemberCanBePrivate.Global - It is used by a child binding through element name
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICommand InvokeUriCommand { get; }
        // ReSharper disable once MemberCanBePrivate.Global - It is used by a child binding through element name
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICommand DeleteActionGroupCommand { get; }
    }
}
