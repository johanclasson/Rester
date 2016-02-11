using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Rester.Control;
using Rester.Model;
using Rester.Service;
using static Rester.Control.Constants;

namespace Rester.ViewModel
{
    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class MainViewModel : ViewModelBase
    {
        private readonly IConfigurationStore _configurationStore;
        private readonly INavigationService _navigationService;
        private readonly IActionInvokerFactory _invokerFactory;
        private readonly IDialog _dialog;
        private readonly IFilePicker _filePicker;

        public MainViewModel(IConfigurationStore configurationStore, INavigationService navigationService, IActionInvokerFactory invokerFactory,
            IDialog dialog, IFilePicker filePicker)
        {
            _configurationStore = configurationStore;
            _navigationService = navigationService; //For some reason, the navigation does not work if not kept as a member
            _invokerFactory = invokerFactory;
            _dialog = dialog;
            _filePicker = filePicker;
            LoadData();
            _dispatcher = Window.Current.Dispatcher;
            InitHandlers();
            Messenger.Default.Register<SomethingIsChangedMessage>(this, async _ => await StoreDataAsync());
            Messenger.Default.Register<UpdateButtonSizeMessage>(this, UpdateButtonSizes);
            NavigateToLogCommand = new RelayCommand(() => _navigationService.NavigateTo(LogPage.Key));
            EditModeCommand = new RelayCommand(() => EditMode = true);
            EditCompletedCommand = new RelayCommand(() => EditMode = false);
            DeleteConfigurationCommand = new RelayCommand<ServiceConfiguration>(async configuration =>
            {
                Configurations.Remove(configuration);
                await StoreDataAsync();
            });
            AddConfigurationCommand = new RelayCommand(AddEmptyServiceConfiguration);
            ExportConfigurationsCommand = new RelayCommand(async () => await ExportConfigurationsAsync());
            ImportConfigurationsCommand = new RelayCommand(async () => await PickFileAndImportSerficeConfigurationsAsync());
            NavigateToAboutPageCommand = new RelayCommand(() => _navigationService.NavigateTo(AboutPage.Key));
        }

        private async void LoadData()
        {
            await LoadDataAsync();
        }

        private Task ExportConfigurationsAsync()
        {
            return _filePicker.CreateTargetFileAsync(async storageFile =>
            {
                await _configurationStore.SaveConfigurationsToFileAsync(Configurations, storageFile);
            });
        }

        private async Task PickFileAndImportSerficeConfigurationsAsync()
        {
            StorageFile storageFile = await _filePicker.PickSingleFileForImportAsync();
            if (storageFile == null)
                return;
            await ImportConfigurationsFromFileAsync(storageFile);
        }

        public async Task ImportConfigurationsFromFileAsync(StorageFile storageFile)
        {
            ServiceConfiguration[] configurations;
            try
            {
                configurations = await _configurationStore.GetConfigurationsFromFileAsync(storageFile);
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"Could not read data because {ex.Message}", "Import Error");
                return;
            }
            string pluralS = configurations.Length > 1 ? "s" : "";
            var message = $"Found configurations for {configurations.Length} service{pluralS}. " +
                          "Do you want to replace the currently configured services, or have the new services added to to them?";
            string answer =
                await _dialog.ShowAsync(message, "File content parsed correctly", new[] {"Add", "Replace", "Cancel"});
            switch (answer)
            {
                case "Replace":
                    Configurations.ClearAndAddRange(configurations);
                    break;
                case "Add":
                    Configurations.AddRange(configurations);
                    break;
            }
            await StoreDataAsync();
        }

        private async void AddEmptyServiceConfiguration()
        {
            Configurations.Add(ServiceConfiguration.CreateSilently("", "", _navigationService, _invokerFactory, EditMode));
            await StoreDataAsync();
        }

        private Task StoreDataAsync()
        {
            return _configurationStore.SaveConfigurationsAsync(Configurations);
        }

        void InitHandlers()
        {
            ApplicationData.Current.DataChanged += async (_, __) =>
            {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    await LoadDataAsync();
                });
            };
        }

        private async Task LoadDataAsync()
        {
            var configs = await _configurationStore.LoadConfigurationsAsync();
            Configurations.ClearAndAddRange(configs);
        }

        private void UpdateButtonSizes(UpdateButtonSizeMessage message)
        {
            double buttonSize = message.Size;
            int maximumActionsInEndpoint = GetMaximumActionsInEndpoint();
            double largestPossibleButtonArray = maximumActionsInEndpoint*(ButtonMaxSize + ButtonMargin) + AdditionalMargin;
            if (largestPossibleButtonArray < message.ColumnWidth)
            {
                buttonSize = ButtonMaxSize;
            }
            var allActions = Configurations.SelectMany(c => c.Endpoints.SelectMany(e => e.Actions));
            foreach (ServiceEndpointAction action in allActions)
            {
                if (Math.Abs(action.ButtonSize - buttonSize) > double.Epsilon)
                    action.ButtonSize = buttonSize;
            }
        }

        private int GetMaximumActionsInEndpoint()
        {
            var actions = Configurations.SelectMany(c => c.Endpoints.Select(e => e.Actions.Count)).ToArray();
            if (!actions.Any())
                return 0;
            return actions.Max();
        }

        public ObservableCollectionWithAddRange<ServiceConfiguration> Configurations { get; } = new ObservableCollectionWithAddRange<ServiceConfiguration>();

        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            private set
            {
                Set(nameof(EditMode), ref _editMode, value);
                Messenger.Default.Send(new EditModeChangedMessage(value));
            }
        }

        private bool _editMode;
        private readonly CoreDispatcher _dispatcher;

        public ICommand NavigateToLogCommand { get; }
        public ICommand EditModeCommand { get; }
        public ICommand EditCompletedCommand { get; }
        public ICommand DeleteConfigurationCommand { get; }
        public ICommand AddConfigurationCommand { get; }
        public ICommand ExportConfigurationsCommand { get; }
        public ICommand ImportConfigurationsCommand { get; }
        public ICommand NavigateToAboutPageCommand { get; }
    }
}