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
                    AllowedCorsOrigins = new List<string>{ "http://localhost:62609", "https://localhost:5001", "https://localhost:44306", "http://localhost:5000" },
                    AllowedScopes = { "ProcessPayment", "GetPayment" }
                }
            };
    }
}