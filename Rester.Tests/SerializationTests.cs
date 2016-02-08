using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Rester.Model;
using Xunit;

// How to use Moq in .NET Core se https://github.com/aspnet/moq4 and http://stackoverflow.com/questions/27918305/mocking-framework-for-asp-net-core-5-0

namespace Rester.Tests
{
    public class SerializationTests : ResterTestBase
    {
        public const string SerializedConfigurations = @"{
  ""Version"": ""0.1"",
  ""Configurations"": [
    {
      ""Name"": ""My configuration name"",
      ""BaseUri"": ""http://baseuri"",
      ""Endpoints"": [
        {
          ""Name"": ""My endpoint name 1"",
          ""Actions"": [
            {
              ""Name"": ""My action name 1"",
              ""Body"": ""My action body 1"",
              ""MediaType"": ""My action content type 1"",
              ""Method"": ""Post 1"",
              ""UriPath"": ""my?action=path1""
            },
            {
              ""Name"": ""My action name 2"",
              ""Body"": ""My action body 2"",
              ""MediaType"": ""My action content type 2"",
              ""Method"": ""Post 2"",
              ""UriPath"": ""my?action=path2""
            }
          ]
        },
        {
          ""Name"": ""My endpoint name 2"",
          ""Actions"": [
            {
              ""Name"": ""My action name 1"",
              ""Body"": ""My action body 1"",
              ""MediaType"": ""My action content type 1"",
              ""Method"": ""Post 1"",
              ""UriPath"": ""my?action=path1""
            }
          ]
        }
      ]
    }
  ]
}";

        [Fact]
        public async void ServiceConfiguration_ShouldBeSerializedToCorrectJson()
        {
            ServiceConfiguration configuration = CreateBuilder()
                .WithEndpoint(1, 2)
                .WithEndpoint(2, 1);
            string result = await Serializer.SerializeAsync(new[] {configuration});
            result.Should().Be(SerializedConfigurations);
        }

        [Fact]
        public async void ASerializedAndCompressedLargeConfiguration_ShouldBeSmallerThanTheRoamingStorageQuota()
        {
            var configurations = BuildArrayOfConfigurations(noConfigs: 10, noEndpoints: 20, noActions: 8);
            string data = await Serializer.SerializeAsync(configurations);
            using (var compressedStream = new MemoryStream())
            {
                await Zipper.WriteCompressedDataToStreamAsync(compressedStream, data);
                int actualNumberOfBytes = compressedStream.ToArray().Length;
                // ApplicationData.RoamingStorageQuota is 100KB. For some reason that property cannot be accessed in the test project.
                actualNumberOfBytes.Should().BeLessThan(100*1024);
            }
        }

        [Fact]
        public async void DeserializingConfigurations_ShouldCreateTheExpectedObjects()
        {
            var configurations = await Deserializer.DeserializeAsync(SerializedConfigurations);
            configurations.Length.Should().Be(1);
            ServiceConfiguration configuration = configurations[0];
            configuration.Name.Should().Be("My configuration name");
            configuration.BaseUri.Should().Be("http://baseuri");
            configuration.Endpoints.Count.Should().Be(2);

            AssertEndpoint(configuration.Endpoints, endpointIndex: 0, noActions: 2);
        }

        private void AssertEndpoint(IList<ServiceEndpoint> endpoints, int endpointIndex, int noActions)
        {
            ServiceEndpoint endpoint = endpoints[endpointIndex];
            endpoint.Name.Should().Be($"My endpoint name {endpointIndex + 1}");
            endpoint.Actions.Count.Should().Be(noActions, "Number of actions");
            for (int i = 0; i < noActions; i++)
            {
                var action = endpoint.Actions[i];
                action.BaseUri.Should().Be("http://baseuri/");
                int actionNumber = i + 1;
                action.Name.Should().Be($"My action name {actionNumber}");
                action.Body.Should().Be($"My action body {actionNumber}");
                action.MediaType.Should().Be($"My action content type {actionNumber}");
                action.Method.Should().Be($"Post {actionNumber}");
                action.UriPath.Should().Be($"my?action=path{actionNumber}");
            }
        }
    }
}