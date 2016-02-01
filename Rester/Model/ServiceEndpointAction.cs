using System;
using GalaSoft.MvvmLight;

namespace Rester.Model
{
    internal class ServiceEndpointAction : AbstractResterModel
    {
        public ServiceEndpointAction()
        {
#if DEBUG
            if (ViewModelBase.IsInDesignModeStatic)
            {
                Name = "My name";
                Body = "Body" + Environment.NewLine + 
                    "Language";
                MediaType = "application/json";
                Method = "Post";
                UriPath = "mypath/jadda/jadda";
            }
#endif
        }

        public ServiceEndpointAction(bool editMode = false) : base(editMode)
        {
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
    }
}