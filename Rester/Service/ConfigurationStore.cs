using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Rester.Model;

namespace Rester.Service
{
    internal interface IConfigurationStore
    {
        Task<ServiceConfiguration[]> LoadConfigurationsAsync();
        Task SaveConfigurationsAsync(IEnumerable<ServiceConfiguration> configurations);
        Task SaveConfigurationsToFileAsync(IEnumerable<ServiceConfiguration> configurations, StorageFile storageFile);
        Task<ServiceConfiguration[]> GetConfigurationsFromFileAsync(StorageFile file);
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class ConfigurationStore : IConfigurationStore
    {
        private const string ResterDbFilename = "rester.rdb";
        private readonly IZipper _zipper;
        private readonly IDeserializer _deserializer;
        private readonly ISerializer _serializer;
        private readonly IDialog _dialog;

        public ConfigurationStore(IZipper zipper, IDeserializer deserializer, ISerializer serializer, IDialog dialog)
        {
            _zipper = zipper;
            _deserializer = deserializer;
            _serializer = serializer;
            _dialog = dialog;
        }

        public async Task<ServiceConfiguration[]> LoadConfigurationsAsync()
        {
            StorageFile file = await GetAvailableStorageFileOrNullAsync();
            if (file == null)
            {
                return new ServiceConfiguration[0];
            }
            try
            {
                return await GetConfigurationsFromFileAsync(file);
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"Could not read syncronized data because {ex.Message}", "Synchronization Error");
                return new ServiceConfiguration[0];
            }
        }

        public async Task<ServiceConfiguration[]> GetConfigurationsFromFileAsync(StorageFile file)
        {
            Stream fileStream = await file.OpenStreamForReadAsync();
            string data = await _zipper.GetDataFromCompressedStreamAsync(fileStream);
            return await _deserializer.DeserializeAsync(data);
        }

        public async Task SaveConfigurationsToFileAsync(IEnumerable<ServiceConfiguration> configurations, StorageFile storageFile)
        {
            string data = await _serializer.SerializeAsync(configurations.ToArray());
            using (Stream stream = await storageFile.OpenStreamForWriteAsync())
            {
                await _zipper.WriteCompressedDataToStreamAsync(stream, data);
            }
        }

        public async Task SaveConfigurationsAsync(IEnumerable<ServiceConfiguration> configurations)
        {
            StorageFile file = await GetOrCreateStorageFile();
            await SaveConfigurationsToFileAsync(configurations, file);
        }

        private async Task<StorageFile> GetOrCreateStorageFile()
        {
            StorageFile file = await GetAvailableStorageFileOrNullAsync();
            if (file != null)
                return file;
            return await ApplicationData.Current.RoamingFolder.CreateFileAsync(ResterDbFilename);
        }

        private async Task<StorageFile> GetAvailableStorageFileOrNullAsync()
        {
            StorageFile file;
            try
            {
                file = await ApplicationData.Current.RoamingFolder.GetFileAsync(ResterDbFilename);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            if (!file.IsAvailable)
            {
                return null;
            }
            return file;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class DesignConfigurationStore : IConfigurationStore
    {
        public Task<ServiceConfiguration[]> LoadConfigurationsAsync()
        {
            var configurations = Enumerable.Range(0, 3).Select(CreateConfiguration).ToList();
            configurations.Insert(0, CreateRealTestData());
            return Task.FromResult(configurations.ToArray());
        }

        public Task SaveConfigurationsAsync(IEnumerable<ServiceConfiguration> configurations)
        {
            return Task.CompletedTask;
        }

        public Task SaveConfigurationsToFileAsync(IEnumerable<ServiceConfiguration> configurations, StorageFile storageFile)
        {
            return Task.CompletedTask;
        }

        public Task<ServiceConfiguration[]> GetConfigurationsFromFileAsync(StorageFile file)
        {
            return Task.FromResult(new ServiceConfiguration[0]);
        }

        private ServiceConfiguration CreateRealTestData()
        {
            var configuration = ServiceConfiguration.CreateSilently("Domoticz", "http://mediacomputer:8080",
                NavigationService, ActionInvokerFactory);
            ServiceAction[] actions =
            {
                ServiceAction.CreateSilently("On", "json.htm?type=command&param=switchlight&idx=1&switchcmd=Set%20Level&level=15", 
                "Get", "", "", () => configuration.BaseUri),
                ServiceAction.CreateSilently("Off", "json.htm?type=command&param=switchlight&idx=1&switchcmd=Off",
                "Get", "", "", () => configuration.BaseUri)
            };
            var actionGroup = ActionGroup.CreateSilently("Dining Room", configuration, NavigationService);
            actionGroup.Actions.AddRange(actions);
            ActionGroup[] groups =
            {
                actionGroup
            };
            configuration.ActionGroups.AddRange(groups);
            return configuration;
        }

        private static IActionInvokerFactory ActionInvokerFactory => SimpleIoc.Default.GetInstance<IActionInvokerFactory>();
        private static INavigationService NavigationService => SimpleIoc.Default.GetInstance<INavigationService>();

        private static ServiceConfiguration CreateConfiguration(int i)
        {
            var configuration = ServiceConfiguration.CreateSilently(
                $"Service Config {i}", "http://myserviceurl:1234",
                NavigationService, ActionInvokerFactory);
            var actions = Enumerable.Range(0, 7).Select(j => CreateAction(() => configuration.BaseUri, j));
            var groups = Enumerable.Range(0, 6).Select(k => CreateGroup(configuration, actions, k));
            configuration.ActionGroups.AddRange(groups);
            return configuration;
        }

        private static ActionGroup CreateGroup(ServiceConfiguration configuration, IEnumerable<ServiceAction> actions, int i)
        {
            var actionGroup = ActionGroup.CreateSilently($"Group {i}", configuration, NavigationService);
            actionGroup.Actions.AddRange(actions);
            return actionGroup;
        }

        private static ServiceAction CreateAction(Func<string> configuration, int i)
        {
            return ServiceAction.CreateSilently($"Action with long name {i}",
                "dostuff?a=b&c=d", "Get", "", "", configuration);
        }
    }
}