using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.IntegrationTests.Utilities.Fakes
{
    internal class StubHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> cannedAnswers;

        public StubHttpMessageHandler(Dictionary<string, HttpResponseMessage> cannedAnswers)
        {
            this.cannedAnswers = cannedAnswers;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (cannedAnswers.ContainsKey($"{request.RequestUri.PathAndQuery}"))
            {
                return await Task.FromResult(cannedAnswers[$"{request.RequestUri.PathAndQuery}"]);
            }
            throw new ArgumentException($"No canned answer exists for query \"{request.Method} {request.RequestUri.PathAndQuery}\"");
        }
    }
}
