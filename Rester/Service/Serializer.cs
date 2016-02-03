using Newtonsoft.Json.Linq;
using Rester.Model;

namespace Rester.Service
{
    public interface ISerializer
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
                JObject jConfig = SerializeServiceConfiguraion(configuration);
                jConfigArray.Add(jConfig);
            }
            return jConfigArray.ToString();
        }

        private static JObject SerializeServiceConfiguraion(ServiceConfiguration configuration)
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
                JObject jEndpoint = SerializeEndpoint(endpoint);
                jEndpointArray.Add(jEndpoint);
            }
            return jConfig;
        }

        private static JObject SerializeEndpoint(ServiceEndpoint endpoint)
        {
            var jActionArray = new JArray();
            var jEndpoint = new JObject
            {
                ["Name"] = endpoint.Name,
                ["Actions"] = jActionArray
            };
            foreach (ServiceEndpointAction action in endpoint.Actions)
            {
                JObject jAction = SerializeAction(action);
                jActionArray.Add(jAction);
            }
            return jEndpoint;
        }

        private static JObject SerializeAction(ServiceEndpointAction action)
        {
            var jAction = new JObject()
            {
                ["Name"] = action.Name,
                ["Body"] = action.Body,
                ["MediaType"] = action.MediaType,
                ["Method"] = action.Method,
                ["UriPath"] = action.UriPath
            };
            return jAction;
        }
    }
}
