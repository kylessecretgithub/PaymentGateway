using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaymentGateway.Gateway.DataAccess;
using PaymentGateway.Gateway.Services;
using System;
using System.Collections.Generic;

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
                ConfigureSwagger(services);
            }
            ConfigureAuth(services);
        }

        protected void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:44392/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:44392/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "ProcessPayment", "Process Payment" },
                                { "GetPayment", "Get Payment" }
                            }
                        }
                    }
                });
            });
        }

        protected virtual void ConfigureAuth(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:44392";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                    options.RequireHttpsMetadata = false;
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProcessPayment", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "ProcessPayment");
                });
                options.AddPolicy("GetPayment", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "GetPayment");
                });
            });
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
