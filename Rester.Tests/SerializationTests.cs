using FluentAssertions;
using Rester.Model;
using Rester.Service;
using Xunit;

// How to use Moq in .NET Core se https://github.com/aspnet/moq4 and http://stackoverflow.com/questions/27918305/mocking-framework-for-asp-net-core-5-0

namespace Rester.Tests
{
    public class SerializationTests : DataFixture
    {
        const string Expected = @"[
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
                .WithEndpoint(index: 1, numberOfActions: 2)
                .WithEndpoint(index: 2, numberOfActions: 1);
            ISerializer serializer = new Serializer();
            string result = serializer.Serialize(new[] {configuration});
            result.Should().Be(Expected);
        }
    }
}