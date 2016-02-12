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

        public ServiceConfigurationBuilder WithGroup(int index, int numberOfActions)
        {
            var actionGroup = ActionGroup.CreateSilently($"My action group name {index}", _configuration,
                _navigationService);
            for (int i = 1; i <= numberOfActions; i++)
            {
                var action = ServiceAction.CreateSilently($"My action name {i}", $"my?action=path{i}",
                    $"Post {i}", $"My action body {i}", $"My action content type {i}", () => _configuration.BaseUri);
                actionGroup.Actions.Add(action);
            }
            _configuration.ActionGroups.Add(actionGroup);
            return this;
        }

        public static implicit operator ServiceConfiguration(ServiceConfigurationBuilder builder)
        {
            return builder._configuration;
        }
    }
}