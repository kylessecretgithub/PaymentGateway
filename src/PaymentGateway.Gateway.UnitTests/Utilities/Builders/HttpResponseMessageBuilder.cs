using System.Net;
using System.Net.Http;
using System.Text;

namespace PaymentGateway.Gateway.UnitTests.Utilities.Builders
{
    internal class HttpResponseMessageBuilder
    {
        private StringContent stringContent = null;
        private HttpStatusCode statusCode = HttpStatusCode.OK;

        internal HttpResponseMessage Build()
        {
            return new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = stringContent
            };
        }

        internal HttpResponseMessageBuilder WithJsonContent(string content)
        {
            stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return this;
        }

        internal HttpResponseMessageBuilder WithHttpStatusCode(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
            return this;
        }
    }
}
