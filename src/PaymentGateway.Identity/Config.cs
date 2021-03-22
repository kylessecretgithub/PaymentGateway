using IdentityServer4.Models;
using System.Collections.Generic;

namespace PaymentGateway.Identity
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("ProcessPayment", "Process Payment"),
                new ApiScope("GetPayment", "Get Payment")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "Merchant",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,                    
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "ProcessPayment", "GetPayment" }
                }
            };
    }
}