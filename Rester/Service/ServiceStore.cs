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
    internal class ServiceStore : IServiceStore
    {
        private const string ResterDbFilename = "rester.db";
        private readonly IZipper _zipper;
        private readonly IDeserializer _deserializer;
        private readonly ISerializer _serializer;
        private readonly IDialogService _dialogService;

        public ServiceStore(IZipper zipper, IDeserializer deserializer, ISerializer serializer, IDialogService dialogService)
        {
            _zipper = zipper;
            _deserializer = deserializer;
            _serializer = serializer;
            _dialogService = dialogService;
        }

        public async Task<ServiceConfiguration[]> LoadServiceConfigurations()
        {
            StorageFile file = await GetAvailableStorageFileOrNullAsync();
            if (file == null)
            {
                return new ServiceConfiguration[0];
            }
            try
            {
                Stream fileStream = await file.OpenStreamForReadAsync();
                string data = await _zipper.GetDataFromCompressedStream(fileStream);
                return await _deserializer.DeserializeAsync(data);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowError($"Could not read syncronized data because {ex.Message}", "Syncronization Error", "Ok", () => {});
                return new ServiceConfiguration[0];
            }
        }

        public async Task SaveServiceConfigurations(IEnumerable<ServiceConfiguration> configurations)
        {
            string data = await _serializer.SerializeAsync(configurations.ToArray());
            StorageFile storageFile = await GetOrCreateStorageFile();
            Stream stream = await storageFile.OpenStreamForWriteAsync();
            await _zipper.WriteCompressedDataToStream(stream, data);
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

    internal class DesignServiceStore : IServiceStore
    {
        public Task<ServiceConfiguration[]> LoadServiceConfigurations()
        {
            var serviceConfigurations = Enumerable.Range(0, 3).Select(CreateServiceConfiguration).ToList();
            serviceConfigurations.Insert(0, CreateRealTestData());
            return Task.FromResult(serviceConfigurations.ToArray());
        }

        public Task SaveServiceConfigurations(IEnumerable<ServiceConfiguration> configurations)
        {
            return Task.CompletedTask;
        }

        private ServiceConfiguration CreateRealTestData()
        {
            var configuration = ServiceConfiguration.CreateSilently("Domoticz", "http://mediamonstret:8070",
                NavigationService, ActionInvokerFactory);
            ServiceEndpointAction[] actions =
            {
                ServiceEndpointAction.CreateSilently("På", "json.htm?type=command&param=switchlight&idx=1&switchcmd=Set%20Level&level=15", 
                "Get", "", "", () => configuration.BaseUri),
                ServiceEndpointAction.CreateSilently("Av", "json.htm?type=command&param=switchlight&idx=1&switchcmd=Off",
                "Get", "", "", () => configuration.BaseUri)
            };
            var serviceEndpoint = ServiceEndpoint.CreateSilently("Matrymmet", configuration, NavigationService);
            serviceEndpoint.Actions.AddRange(actions);
            ServiceEndpoint[] endpoints =
            {
                serviceEndpoint
            };
            configuration.Endpoints.AddRange(endpoints);
            return configuration;
        }

        private static IActionInvokerFactory ActionInvokerFactory => SimpleIoc.Default.GetInstance<IActionInvokerFactory>();
        private static INavigationService NavigationService => SimpleIoc.Default.GetInstance<INavigationService>();

        private static ServiceConfiguration CreateServiceConfiguration(int i)
        {
            var configuration = ServiceConfiguration.CreateSilently(
                $"Service Config {i}", "http://myserviceurl:1234",
                NavigationService, ActionInvokerFactory);
            var actions = Enumerable.Range(0, 7).Select(j => CreateAction(() => configuration.BaseUri, j));
            var endpoints = Enumerable.Range(0, 6).Select(k => CreateEndpoint(configuration, actions, k));
            configuration.Endpoints.AddRange(endpoints);
            return configuration;
        }

        private static ServiceEndpoint CreateEndpoint(ServiceConfiguration configuration, IEnumerable<ServiceEndpointAction> actions, int i)
        {
            var serviceEndpoint = ServiceEndpoint.CreateSilently($"Endpoint {i}", configuration, NavigationService);
            serviceEndpoint.Actions.AddRange(actions);
            return serviceEndpoint;
        }

        private static ServiceEndpointAction CreateAction(Func<string> configuration, int i)
        {
            return ServiceEndpointAction.CreateSilently($"Action with long name {i}",
                "dostuff?a=b&c=d", "Get", "", "", configuration);
        }
    }

    internal interface IServiceStore
    {
        Task<ServiceConfiguration[]> LoadServiceConfigurations();
        Task SaveServiceConfigurations(IEnumerable<ServiceConfiguration> configurations);
    }
}