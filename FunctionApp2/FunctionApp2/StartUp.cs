using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(FunctionApp2.StartUp))]
namespace FunctionApp2
{
    public class StartUp : FunctionsStartup
    {
        private ILoggerFactory _loggerFactory;
        private IConfigurationRoot _configurationRoot;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _configurationRoot = new ConfigurationBuilder().AddJsonFile("local.settings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            //builder.Services.AddLogging();
            ConfigureServices(builder);
        }

        public void ConfigureServices(IFunctionsHostBuilder builder)
        {
            _loggerFactory = new LoggerFactory();
            var logger = _loggerFactory.CreateLogger("Startup");
            logger.LogInformation("Got Here in Startup");
            //Do something with builder
            //
            Uri uri = new Uri("http://taco-randomizer.herokuapp.com/random/");
            var httpClient = new HttpClient();
            httpClient.BaseAddress = uri;
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string connectionStr = _configurationRoot["ConnectionStrings:Default"];
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionStr);
            var recipeClient = new RecipeClient(logger, httpClient, blobServiceClient);
            builder.Services.TryAddSingleton<IRecipeClient>(recipeClient);
        }

        public StartUp()
        {
        }
    }
}