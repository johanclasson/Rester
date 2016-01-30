using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rester.Model;

namespace Rester.Service
{
    internal class LogStore : ILogStore
    {
        public Task<bool> GetOnlyFromToday()
        {
            throw new NotImplementedException();
        }

        public Task SetOnlyFromToday(bool value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponse[]> GetLogEntries()
        {
            throw new NotImplementedException();
        }
    }

    internal class DesignLogStore : ILogStore
    {
        private bool OnlyFromToday { get; set; } = true;

        public Task<bool> GetOnlyFromToday()
        {
            return Task.FromResult(OnlyFromToday);
        }

        public Task SetOnlyFromToday(bool value)
        {
            OnlyFromToday = value;
            return Task.CompletedTask;
        }

        public Task<HttpResponse[]> GetLogEntries()
        {
            var entries = new List<HttpResponse>();
            bool isSuccessfulStatusCode = true;
            var numberOfEntries = OnlyFromToday ? 5 : 20;
            for (var i = 0; i < numberOfEntries; i++)
            {
                isSuccessfulStatusCode = !isSuccessfulStatusCode;
                entries.Add(new HttpResponse
                {
                    Content = @"jadda jadda
pradda fladda",
                    ReasonPhrase = "OK",
                    TimeToResponse = TimeSpan.FromMilliseconds(123.9),
                    StatusCode = 200,
                    Method = "Get",
                    Uri = "www.somedomain.se/somepath",
                    CallTime = DateTime.Now,
                    IsSuccessfulStatusCode = isSuccessfulStatusCode
                });
            }
            return Task.FromResult(entries.ToArray());
        }
    }

    internal interface ILogStore
    {
        Task<bool> GetOnlyFromToday();
        Task SetOnlyFromToday(bool value);
        Task<HttpResponse[]> GetLogEntries();
    }
}