using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.FakeBankApi
{
    public class BankResponse
    { 
        public long? PaymentId { get; set; }
        public string DetailedMessage { get; set; }
        public string Status { get; set; }
    }
}
