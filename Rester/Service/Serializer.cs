using Newtonsoft.Json.Linq;
using Rester.Model;

namespace Rester.Service
{
    internal interface ISerializer
    {
        string Serialize(ServiceConfiguration[] configurations);
    }

    internal class Serializer : ISerializer
    {
        public string Serialize(ServiceConfiguration[] configurations)
        {
            var jConfigArray = new JArray();
            foreach (ServiceConfiguration configuration in configurations)
            {
                var jEndpointArray = new JArray();
                var jConfig = new JObject
                {
                    ["Name"] = configuration.Name,
                    ["BaseUri"] = configuration.BaseUri,
                    ["Endpoints"] = jEndpointArray
                };
                foreach (ServiceEndpoint endpoint in configuration.Endpoints)
                {
                    var jActionArray = new JArray();
                    var jEndpoint = new JObject
                    {
                        ["Name"] = endpoint.Name,
                        ["Actions"] = jActionArray
                    };
                    foreach (ServiceEndpointAction action in endpoint.Actions)
                    {
                        var jAction = new JObject()
                        {
                            ["Name"] = action.Name,
                            ["Body"] = action.Body,
                            ["MediaType"] = action.MediaType,
                            ["Method"] = action.Method,
                            ["UriPath"] = action.UriPath
                        };
                        jActionArray.Add(jAction);
                    }
                    jEndpointArray.Add(jEndpoint);
                }
                jConfigArray.Add(jConfig);
            }
            return jConfigArray.ToString();
        }
    }
}
