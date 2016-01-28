using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Rester.Model;

namespace Rester.ViewModel
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
            var configuration = new ServiceConfiguration
            {
                Name = "Domoticz",
                BaseUri = "http://mediamonstret:8070"
            };
            ServiceEndpointAction[] actions =
            {
                new ServiceEndpointAction(configuration)
                {
                    Name = "På",
                    UriPath = "json.htm?type=command&param=switchlight&idx=1&switchcmd=On",
                    Method = "Get"
                },
                new ServiceEndpointAction(configuration)
                {
                    Name = "Av",
                    UriPath = "json.htm?type=command&param=switchlight&idx=1&switchcmd=Off",
                    Method = "Get"
                }
            };
            ServiceEndpoint[] endpoints =
            {
                new ServiceEndpoint(actions)
                {
                    Name = "Matrummet",
                    Symbol = Symbol.Flag
                }
            };
            configuration.Endpoints.AddRange(endpoints);
            return configuration;
        }

        private static ServiceConfiguration CreateServiceConfiguration(int i)
        {
            var configuration = new ServiceConfiguration()
            {
                Name = $"Service Config {i}",
                BaseUri = "http://myserviceurl:1234"
            };
            var actions = Enumerable.Range(0, 7).Select(j => CreateAction(j, configuration));
            var endpoints = Enumerable.Range(0, 6).Select(k => CreateEndpoint(actions, k));
            configuration.Endpoints.AddRange(endpoints);
            return configuration;
        }

        private static ServiceEndpoint CreateEndpoint(IEnumerable<ServiceEndpointAction> actions, int i)
        {
            return new ServiceEndpoint(actions)
            {
                Symbol =  Symbol.Clock,
                Name = $"Endpoint {i}"
            };
        }

        private static ServiceEndpointAction CreateAction(int i, ServiceConfiguration configuration)
        {
            return new ServiceEndpointAction(configuration)
            {
                Name = $"Action {i}",
                UriPath = "dostuff?a=b&c=d"
            };
        }
    }

    internal interface IServiceStore
    {
        Task<ServiceConfiguration[]> GetServiceConfigurations();
    }
}