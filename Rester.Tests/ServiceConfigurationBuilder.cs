using GalaSoft.MvvmLight.Views;
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
            _configuration = ServiceConfiguration.CreateSilently(
                "My configuration name", "http://baseuri",
                _navigationService, invokerFactory);
        }

        public ServiceConfigurationBuilder WithEndpoint(int index, int numberOfActions)
        {
            var endpoint = ServiceEndpoint.CreateSilently($"My endpoint name {index}", _configuration,
                _navigationService);
            for (int i = 1; i <= numberOfActions; i++)
            {
                var action = ServiceEndpointAction.CreateSilently($"My action name {i}", $"my?action=path{i}",
                    $"Post {i}", $"My action body {i}", $"My action content type {i}", () => _configuration.BaseUri);
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