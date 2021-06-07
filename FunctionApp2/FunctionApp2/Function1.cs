using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Ajax.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp2
{
    public class Function1
    {
        private readonly IRecipeClient _recipeClient;
        public Function1(IRecipeClient recipeClient)
        {
            _recipeClient = recipeClient;
        }

        [FunctionName("Function1")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Uri uri = new Uri("http://taco-randomizer.herokuapp.com/random/");
            string recipe = await context.CallActivityAsync<string>("DownloadRecipe", null);
            string saverecipe = await context.CallActivityAsync<string>("SaveRecipe", recipe);

            return recipe + saverecipe;
        }


        [FunctionName("Function1_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function1", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("DownloadRecipe")]

        public async Task<string> DownloadRecipe([ActivityTrigger] ILogger logger)
        {
            return await _recipeClient.DownloadRecipe();
        }

        [FunctionName("SaveRecipe")]
        public async Task SaveRecipe([ActivityTrigger] string recipe)

        {
            await _recipeClient.SaveRecipe(recipe);
        }
    }
}