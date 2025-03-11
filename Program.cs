using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SanfordTest;
using SanfordTest.Configurations;
using System.IO;
using System;
using System.Threading.Tasks;
using Auth0.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using static System.Net.WebRequestMethods;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.UseFunctionsAuthorization();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var appSettings = config.GetSection("Values").Get<AppSettings>();
        services.AddSingleton<AppSettings>(appSettings);
        services.AddSingleton<ICsvWriter, CsvWriterService>();

        var auth0Domain = config["Auth0:Domain"];
        var auth0Audience = config["Auth0:ApiIdentifier"];

        services.AddFunctionsAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtFunctionsBearer(options =>
        {
            options.Authority = auth0Domain;
            options.Audience = auth0Audience;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = auth0Domain,
                ValidAudience = auth0Audience
            };
        });
    })
    .Build();

host.Run();

