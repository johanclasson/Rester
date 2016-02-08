using System.Collections.Generic;
using GalaSoft.MvvmLight.Views;
using Moq;
using Rester.Model;
using Rester.Service;

namespace Rester.Tests
{
    public abstract class ResterTestBase
    {
        private IZipper _zipper;
        private IDeserializer _deserializer ;
        protected IDeserializer Deserializer => _deserializer ?? (_deserializer = new Deserializer(NavigationServiceMock, InvokerFactoryMock));
        private ISerializer _serializer ;
        protected ISerializer Serializer => _serializer ?? (_serializer = new Serializer());
        protected IZipper Zipper => _zipper ?? (_zipper = new Zipper());

        private INavigationService _navigationService;
        private INavigationService NavigationServiceMock => _navigationService ?? (_navigationService = Mock.Of<INavigationService>());

        private IActionInvokerFactory _invokerFactory;
        private IActionInvokerFactory InvokerFactoryMock => _invokerFactory ?? (_invokerFactory = Mock.Of<IActionInvokerFactory>());

        protected ServiceConfigurationBuilder CreateBuilder()
        {
            return new ServiceConfigurationBuilder(NavigationServiceMock, InvokerFactoryMock);
        }

        protected ServiceConfiguration[] BuildArrayOfConfigurations(int noConfigs, int noEndpoints, int noActions)
        {
            var configurations = new List<ServiceConfiguration>();
            for (int configIndex = 0; configIndex < noConfigs; configIndex++)
            {
                ServiceConfigurationBuilder builder = CreateBuilder();
                for (int endpointIndex = 0; endpointIndex < noEndpoints; endpointIndex++)
                {
                    builder.WithEndpoint(endpointIndex, noActions);
                }
                configurations.Add(builder);
            }
            return configurations.ToArray();
        }
    }
}