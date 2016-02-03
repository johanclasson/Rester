using GalaSoft.MvvmLight.Views;
using Moq;
using Rester.Model;
using Rester.Service;

namespace Rester.Tests
{
    public class ServiceConfigurationBuilder
    {
        private readonly INavigationService _navigationService;
        private readonly ServiceConfiguration _configuration;

        public ServiceConfigurationBuilder(INavigationService navigationService, IActionInvokerFactory invokerFactory)
        {
            _navigationService = navigationService;
            _configuration = new ServiceConfiguration(_navigationService, invokerFactory)
            {
                BaseUri = "http://baseuri",
                Name = "My configuration name"
            };
        }

        public ServiceConfigurationBuilder WithEndpoint(int index, int numberOfActions)
        {
            var endpoint = new ServiceEndpoint(_configuration, _navigationService)
            {
                Name = $"My endpoint name {index}"
            };
            for (int i = 1; i <= numberOfActions; i++)
            {
                var action = new ServiceEndpointAction(() => _configuration.BaseUri)
                {
                    Name = $"My action name {i}",
                    Body = $"My action body {i}",
                    MediaType = $"My action content type {i}",
                    Method = $"Post {i}",
                    UriPath = $"my?action=path{i}"
                };
                endpoint.Actions.Add(action);
            }
            _configuration.Endpoints.Add(endpoint);
            return this;
        }

        public static implicit operator ServiceConfiguration(ServiceConfigurationBuilder builder)
        {
            return builder._configuration;
        }
    }
}