using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace lambda_dotnetConfiguration
{
    public class Functions
    {
        static IServiceProvider services;
        
        public static Func<IServiceProvider> ConfigureServices = () =>
        {
            var serviceCollection = new ServiceCollection();
            // serviceCollection.AddOptions();

            //building configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddSystemsManager("/AppName/Secrets")  // Parameter Store for app secrets
                .Build();

            // Bind config <T>
            var appSecrets = configuration.Get<AppKeys>();
            var appSettings = configuration.GetSection("Section1").Get<AppConfig>();

            serviceCollection.AddSingleton(appSecrets);
            serviceCollection.AddSingleton(appSettings);

            return serviceCollection.BuildServiceProvider();
        };
        
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        static Functions()
        {
            services = ConfigureServices();
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of blogs</returns>
        public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");

            Console.WriteLine("Request: " + JsonConvert.SerializeObject(request));
            Console.WriteLine("Context: " + JsonConvert.SerializeObject(context));

            try {
                Console.WriteLine("Config: " + JsonConvert.SerializeObject(services.GetRequiredService<AppConfig>()));
                Console.WriteLine("Keys: " + JsonConvert.SerializeObject(services.GetRequiredService<AppKeys>()));
            } catch (Exception ex) { Console.WriteLine("Error outputting config:" + ex.Message); }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "Hello AWS Serverless",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }
    }
}
