using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rester.Model;

namespace Rester.Service
{
    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class LogStore : ILogStore
    {
        private bool OnlyFromToday { get; set; }
        private List<HttpResponse> Logs { get; } = new List<HttpResponse>(); 

        public Task<bool> GetOnlyFromTodayAsync()
        {
            return Task.FromResult(OnlyFromToday);
        }

        public Task SetOnlyFromTodayAsync(bool value)
        {
            OnlyFromToday = value;
            return Task.CompletedTask;
        }

        public Task<HttpResponse[]> GetLogEntriesAsync()
        {
            return Task.FromResult(Logs.ToArray());
        }

        public Task AddAsync(HttpResponse logEntry)
        {
            Logs.Insert(0, logEntry);
            return Task.CompletedTask;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class DesignLogStore : ILogStore
    {
        private bool OnlyFromToday { get; set; } = true;

        public Task<bool> GetOnlyFromTodayAsync()
        {
            return Task.FromResult(OnlyFromToday);
        }

        public Task SetOnlyFromTodayAsync(bool value)
        {
            OnlyFromToday = value;
            return Task.CompletedTask;
        }

        public Task<HttpResponse[]> GetLogEntriesAsync()
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
jadda jadda",
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

        public Task AddAsync(HttpResponse logEntry)
        {
            return Task.CompletedTask;
        }
    }

    internal interface ILogStore
    {
        Task<bool> GetOnlyFromTodayAsync();
        Task SetOnlyFromTodayAsync(bool value);
        Task<HttpResponse[]> GetLogEntriesAsync();
        Task AddAsync(HttpResponse logEntry);
    }
}