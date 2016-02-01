using System;
using GalaSoft.MvvmLight;

namespace Rester.Model
{
    internal class ServiceEndpointAction : AbstractResterModel
    {
#if DEBUG
        [Obsolete("Constructor is only present for design purposes")]
        public ServiceEndpointAction()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                Name = "My name";
                Body = "Body" + Environment.NewLine + 
                    "Language";
                MediaType = "application/json";
                Method = "Post";
                UriPath = "mypath/jadda/jadda";
                GetBaseUri = () => "http://myservice.com:1234";
            }
        }
#endif

        public ServiceEndpointAction(Func<string> getBaseUri, bool editMode = false) : base(editMode)
        {
            GetBaseUri = getBaseUri;
        }

        public string UriPath { get { return _uriPath; } set { Set(nameof(UriPath), ref _uriPath, value); } }
        private string _uriPath;

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public string Method { get { return _method; } set { Set(nameof(Method), ref _method, value); } }
        private string _method;

        public string Body { get { return _body; } set { Set(nameof(Body), ref _body, value); } }
        private string _body;

        public string MediaType { get { return _mediaType; } set { Set(nameof(MediaType), ref _mediaType, value); } }
        private string _mediaType;

        public bool Processing { get; set; }

        private Func<string> GetBaseUri { get; }

        public string BaseUri
        {
            get
            {
                string baseUri = GetBaseUri().Trim();
                if (baseUri.EndsWith("/"))
                    return baseUri;
                return baseUri + "/";
            }
        }
    }
}