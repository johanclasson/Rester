﻿using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Rester.Control;

namespace Rester.Model
{
    public class ServiceEndpointAction : AbstractResterModel
    {
#if DEBUG
        [Obsolete("Constructor is only present for design purposes")]
        // ReSharper disable once UnusedMember.Global - Constructor is only present for design purposes
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

        public static ServiceEndpointAction CreateSilently(string name, string uriPath, string method,
            string body, string mediaType, Func<string> getBaseUri, bool editMode = false)
        {
            return new ServiceEndpointAction(getBaseUri, editMode)
            {
                _uriPath = uriPath,
                _name = name,
                _method = method,
                _body = body,
                _mediaType = mediaType
            };
        }

        private ServiceEndpointAction(Func<string> getBaseUri, bool editMode) : base(editMode)
        {
            GetBaseUri = getBaseUri;
            Messenger.Default.Register<UpdateButtonSizeMessage>(this, message => ButtonSize = message.Size);
        }

        public string Name { get { return _name; } set { SetAndSave(nameof(Name), ref _name, value); } }
        private string _name;

        public string UriPath { get { return _uriPath; } set { SetAndSave(nameof(UriPath), ref _uriPath, value); } }
        private string _uriPath;

        public string Method { get { return _method; } set { SetAndSave(nameof(Method), ref _method, value); } }
        private string _method;

        public string Body { get { return _body; } set { SetAndSave(nameof(Body), ref _body, value); } }
        private string _body;

        public string MediaType { get { return _mediaType; } set { SetAndSave(nameof(MediaType), ref _mediaType, value); } }
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

        public double ButtonSize { get { return _buttonSize; } set { Set(nameof(ButtonSize), ref _buttonSize, value); } }
        private double _buttonSize = 100;

    }
}