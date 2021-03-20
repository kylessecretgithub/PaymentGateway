using PaymentGateway.Gateway.UnitTests.Utilities.Fakes;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PaymentGateway.Gateway.UnitTests.Utilities.Builders
{
    internal class HttpClientBuilder
    {
        private Dictionary<string, HttpResponseMessage> cannedAnswers;
        private StubHttpMessageHandler messageHandler;

        internal HttpClientBuilder()
        {
            cannedAnswers = new Dictionary<string, HttpResponseMessage>();
        }

        internal HttpClientBuilder WithMessageHandler(StubHttpMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
            return this;
        }

        internal HttpClient Build()
        {
            if (messageHandler == null)
            {
                messageHandler = new StubHttpMessageHandler(cannedAnswers);
            }
            return new HttpClient(messageHandler)
            {
                BaseAddress = new Uri("https://FakeBank.com")
            };
        }
    }
}
