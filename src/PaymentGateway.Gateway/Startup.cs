using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PaymentGateway.Gateway.Controllers.v1;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Services;
using System;

namespace PaymentGateway.Gateway
{
    public class Startup
    {
        private readonly IWebHostEnvironment currentEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            currentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<PaymentController>();
            services.AddScoped<PaymentService>();
            services.AddScoped<ReportingService>();
            services.AddScoped<PaymentsRepository>();
            services.AddDbContextPool<PaymentGatewayContext>(options =>
               options.UseInMemoryDatabase(Configuration.GetConnectionString("ReportingDatabaseConnectionString")));
            string bankEndpoint = Configuration.GetConnectionString("BankConnectionString");
            services.AddHttpClient<BankFacade>(client =>
            {
                client.BaseAddress = new Uri(bankEndpoint);
            });
            if (currentEnvironment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway", Version = "v1" });
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
