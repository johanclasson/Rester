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
            var serviceConfigurations = Enumerable.Range(0, 3).Select(i => CreateServiceConfiguration(endpoints, i));
            return Task.FromResult(serviceConfigurations.ToArray());
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