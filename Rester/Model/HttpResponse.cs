using System;
using GalaSoft.MvvmLight;

namespace Rester.Model
{
    internal class HttpResponse : ObservableObject
    {
        public int? StatusCode { get { return _statusCode; } set { Set(nameof(StatusCode), ref _statusCode, value); } }
        private int? _statusCode;

        public string ReasonPhrase { get { return _reasonPhrase; } set { Set(nameof(ReasonPhrase), ref _reasonPhrase, value); } }
        private string _reasonPhrase;

        public TimeSpan TimeToResponse { get { return _timeToResponse; } set { Set(nameof(TimeToResponse), ref _timeToResponse, value); } }
        private TimeSpan _timeToResponse;

        public string Content { get { return _content; } set { Set(nameof(Content), ref _content, value); } }
        private string _content;

        public DateTime CallTime { get { return _callTime; } set { Set(nameof(CallTime), ref _callTime, value); } }
        private DateTime _callTime;

        public string Uri { get { return _uri; } set { Set(nameof(Uri), ref _uri, value); } }
        private string _uri;

        public string Method { get { return _method; } set { Set(nameof(Method), ref _method, value); } }
        private string _method;

        public bool IsSuccessfulStatusCode { get { return _isSuccessfulStatusCode; } set { Set(nameof(IsSuccessfulStatusCode), ref _isSuccessfulStatusCode, value); } }
        private bool _isSuccessfulStatusCode;
    }
}