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
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Uri uri = new Uri("http://taco-randomizer.herokuapp.com/random/");
            string recipe = await context.CallActivityAsync<string>("DownloadRecipe", uri);
            string saverecipe= await context.CallActivityAsync<string>("SaveRecipe", recipe);
            
            return recipe + saverecipe;
        }


        [FunctionName("Function1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
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
        
        public static async Task<string> DownloadRecipe([ActivityTrigger]  Uri uri,ILogger log)
        {
            System.Net.Http.HttpClient client = new HttpClient();
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("");
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                log.LogInformation(result);
                return result;

            }
            return "";
        }
        
       [FunctionName("SaveRecipe")]
        public static async Task<string> SaveRecipe([ActivityTrigger] string recipe)
            
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=azurefunctionstest202106;AccountKey=HIwwoHfRvNOF3ZvjJ0iV4dVsIqLeKkYi+YOl4rbYihd7EJHhl93FeKC/iArKC4uaYzLgVeGIvAmWVZUYZFGDLQ==;EndpointSuffix=core.windows.net");
            BlobContainerClient blobContainerClient =  blobServiceClient.GetBlobContainerClient("recipes");

            BlobClient blobClient = blobContainerClient.GetBlobClient("recipe_"+ Guid.NewGuid().ToString()+".json");


            await blobClient.UploadAsync(recipe.ToStream());

            return "OK";
        }

        public static Stream ToStream(this string str)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(str);
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}