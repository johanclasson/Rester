using GalaSoft.MvvmLight.Views;
using Moq;
using Rester.Model;
using Rester.Service;
using Xunit;

// How to use Moq in .NET Core se https://github.com/aspnet/moq4 and http://stackoverflow.com/questions/27918305/mocking-framework-for-asp-net-core-5-0

namespace Rester.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {
            ISerializer serializer = new Serializer();
            var navigationService = Mock.Of<INavigationService>();
            var invokerFactory = Mock.Of<IActionInvokerFactory>();

            var configuration = new ServiceConfiguration(navigationService, invokerFactory);
            var endpoint = new ServiceEndpoint(configuration, navigationService);
            endpoint.Actions.Add(new ServiceEndpointAction(() => "http://baseuri"));
            configuration.Endpoints.Add(endpoint);
            serializer.Serialize(new[] {configuration});
        }
    }
}