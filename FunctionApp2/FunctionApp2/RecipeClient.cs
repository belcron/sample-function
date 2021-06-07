using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp2
{
    public class RecipeClient : IRecipeClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public RecipeClient(ILogger logger, HttpClient httpClient, BlobServiceClient blobServiceClient)
        {
            _httpClient = httpClient;
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        public string ContainerName { get; set; } = "recipes";
        

        public async Task<string> DownloadRecipe()
        {
            //_logger.LogInformation("Downloading recipe.");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await _httpClient.GetAsync("");
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(result);
                return result;

            }
            return "";
        }


        public async Task SaveRecipe(string recipe)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            string recipeName = "recipe_" + Guid.NewGuid().ToString() + ".json";
            _logger.LogInformation(recipeName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(recipeName);

            await blobClient.UploadAsync(recipe.ToStream());
        }
    }
}
