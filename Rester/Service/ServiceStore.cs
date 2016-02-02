using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Rester.Model;

namespace Rester.Service
{
    internal class ServiceStore : IServiceStore
    {
        public Task<ServiceConfiguration[]> GetServiceConfigurations()
        {
            throw new System.NotImplementedException();
        }
    }

    internal class DesignServiceStore : IServiceStore
    {
        public Task<ServiceConfiguration[]> GetServiceConfigurations()
        {
            var serviceConfigurations = Enumerable.Range(0, 3).Select(CreateServiceConfiguration).ToList();
            serviceConfigurations.Insert(0, CreateRealTestData());
            return Task.FromResult(serviceConfigurations.ToArray());
        }

        private ServiceConfiguration CreateRealTestData()
        {
            var configuration = new ServiceConfiguration(NavigationService, ActionInvokerFactory)
            {
                Name = "Domoticz",
                BaseUri = "http://mediamonstret:8070"
            };
            ServiceEndpointAction[] actions =
            {
                new ServiceEndpointAction(() => configuration.BaseUri)
                {
                    Name = "På",
                    UriPath = "json.htm?type=command&param=switchlight&idx=1&switchcmd=Set%20Level&level=15",
                    Method = "Get"
                },
                new ServiceEndpointAction(() => configuration.BaseUri)
                {
                    Name = "Av",
                    UriPath = "json.htm?type=command&param=switchlight&idx=1&switchcmd=Off",
                    Method = "Get"
                }
            };
            var serviceEndpoint = new ServiceEndpoint(configuration, NavigationService) {Name = "Matrummet"};
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
            var configuration = new ServiceConfiguration(NavigationService, ActionInvokerFactory)
            {
                Name = $"Service Config {i}",
                BaseUri = "http://myserviceurl:1234"
            };
            var actions = Enumerable.Range(0, 7).Select(j => CreateAction(() => configuration.BaseUri, j));
            var endpoints = Enumerable.Range(0, 6).Select(k => CreateEndpoint(configuration, actions, k));
            configuration.Endpoints.AddRange(endpoints);
            return configuration;
        }

        private static ServiceEndpoint CreateEndpoint(ServiceConfiguration configuration, IEnumerable<ServiceEndpointAction> actions, int i)
        {
            var serviceEndpoint = new ServiceEndpoint(configuration, NavigationService) { Name = $"Endpoint {i}" };
            serviceEndpoint.Actions.AddRange(actions);
            return serviceEndpoint;
        }

        private static ServiceEndpointAction CreateAction(Func<string> configuration, int i)
        {
            return new ServiceEndpointAction(configuration)
            {
                Name = $"Action with long name {i}",
                UriPath = "dostuff?a=b&c=d",
                Method = "Get",
                Body = "",
                MediaType = ""
            };
        }
    }

    internal interface IServiceStore
    {
        Task<ServiceConfiguration[]> GetServiceConfigurations();
    }
}