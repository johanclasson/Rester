using System.Collections.Generic;
using Rester.Model;
using Rester.Service;

namespace Rester.Tests
{
    public abstract class ResterTestBase
    {
        private ISerializer _serializer ;
        private IZipper _zipper;
        protected ISerializer Serializer => _serializer ?? (_serializer = new Serializer());
        protected IZipper Zipper => _zipper ?? (_zipper = new Zipper());

        protected ServiceConfigurationBuilder CreateBuilder()
        {
            return new ServiceConfigurationBuilder();
        }

        protected ServiceConfiguration[] BuildArrayOfServiceConfigurations(int noConfigs, int noEndpoints, int noActions)
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