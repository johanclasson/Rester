﻿using GalaSoft.MvvmLight;
using Rester.Service;
using Rester.ViewModel;

namespace Rester.Model
{
    internal class ServiceConfiguration : ObservableObject
    {
        public ServiceConfiguration()
        {
            Endpoints = new ObservableCollectionWithAddRange<ServiceEndpoint>();
        }

        public string BaseUri { get { return _baseUri; } set { Set(nameof(BaseUri), ref _baseUri, value); } }
        private string _baseUri;

        public string Name { get { return _name; } set { Set(nameof(Name), ref _name, value); } }
        private string _name;

        public ObservableCollectionWithAddRange<ServiceEndpoint> Endpoints { get; }
    }
}
