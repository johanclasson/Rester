using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var actions = Enumerable.Range(0, 7).Select(CreateAction);
            var endpoints = Enumerable.Range(0, 6).Select(i => CreateEndpoint(actions, i));
            var serviceConfigurations = Enumerable.Range(0, 3).Select(i => CreateServiceConfiguration(endpoints, i)).ToList();
            serviceConfigurations.Insert(0, CreateRealTestData());
            return Task.FromResult(serviceConfigurations.ToArray());
        }

        private ServiceConfiguration CreateRealTestData()
        {
            ServiceEndpointAction[] actions =
            {
                new ServiceEndpointAction
                {
                    Name = "På",
                    Uri = "json.htm?type=command&param=switchlight&idx=1&switchcmd=On"
                },
                new ServiceEndpointAction
                {
                    Name = "Av",
                    Uri = "json.htm?type=command&param=switchlight&idx=1&switchcmd=Off"
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
            return new ServiceConfiguration(endpoints)
            {
                Name = "Domoticz",
                UriRoot = "http://mediamonstret:8070"
            };
        }

        private static ServiceConfiguration CreateServiceConfiguration(IEnumerable<ServiceEndpoint> endpoints, int i)
        {
            return new ServiceConfiguration(endpoints)
            {
                Name = $"Service Config {i}",
                UriRoot = "http://myserviceurl:1234"
            };
        }

        private static ServiceEndpoint CreateEndpoint(IEnumerable<ServiceEndpointAction> actions, int i)
        {
            return new ServiceEndpoint(actions)
            {
                Symbol =  Symbol.Clock,
                Name = $"Endpoint {i}"
            };
        }

        private static ServiceEndpointAction CreateAction(int i)
        {
            return new ServiceEndpointAction
            {
                Name = $"Action {i}",
                Uri = "dostuff?a=b&c=d"
            };
        }
    }

    public interface IServiceStore
    {
        Task<ServiceConfiguration[]> GetServiceConfigurations();
    }
}