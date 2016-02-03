using System.IO;
using FluentAssertions;
using Rester.Model;
using Xunit;

// How to use Moq in .NET Core se https://github.com/aspnet/moq4 and http://stackoverflow.com/questions/27918305/mocking-framework-for-asp-net-core-5-0

namespace Rester.Tests
{
    public class SerializationTests : ResterTestBase
    {
        public const string Expected = @"[
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
]";

        [Fact]
        public void ServiceConfiguration_ShouldBeSerializedToCorrectJson()
        {
            ServiceConfiguration configuration = CreateBuilder()
                .WithEndpoint(1, 2)
                .WithEndpoint(2, 1);
            var result = Serializer.Serialize(new[] { configuration });
            result.Should().Be(Expected);
        }

        [Fact]
        public void ASerializedAndCompressedLargeConfiguration_ShouldBeSmallerThanTheRoamingStorageQuota()
        {
            var configurations = BuildArrayOfServiceConfigurations(noConfigs: 10, noEndpoints: 20, noActions: 8);
            var data = Serializer.Serialize(configurations);
            using (var compressedStream = new MemoryStream())
            {
                Zipper.WriteCompressedDataToStream(compressedStream, data);
                int actualNumberOfBytes = compressedStream.ToArray().Length;
                // ApplicationData.RoamingStorageQuota is 100KB. For some reason that property cannot be accessed in the test project.
                actualNumberOfBytes.Should().BeLessThan(100*1024);
            }
        }

        //TODO: Deserialize
    }
}