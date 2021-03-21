using Newtonsoft.Json;
using PaymentGateway.Gateway.Models;

namespace PaymentGateway.Gateway.UnitTests.Utilities.Builders
{
    internal class BankResponseBuilder
    {
        private string status = "Processed";
        private string detailedMessage = "Processed payment sucessfully";
        private long? paymentId = 1;

        internal string BuildJson()
        {
            return JsonConvert.SerializeObject(
                new BankResponse
                {
                    PaymentId = paymentId,
                    Status = status,
                    DetailedMessage = detailedMessage
                });
        }

        internal BankResponse Build()
        {
            return new BankResponse
            {
                PaymentId = paymentId,
                Status = status,
                DetailedMessage = detailedMessage
            };
        }

        internal BankResponseBuilder WithDetailedMessage(string detailedMessage)
        {
            this.detailedMessage = detailedMessage;
            return this;
        }

        internal BankResponseBuilder WithStatus(string status)
        {
            this.status = status;
            return this;
        }

        internal BankResponseBuilder WithPaymentId(long? paymentId)
        {
            this.paymentId = paymentId;
            return this;
        }
    }
}
