using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentGateway.Gateway.IntegrationTests.Utilities.Fakes
{
    public class FakeStartup : Startup
    {

        public FakeStartup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {

        }

        protected override void ConfigureAuth(IServiceCollection services)
        {

        }
    }
}
