using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace SampleAzureFunction
{
    public static class Employee
    {
        [FunctionName("Employee")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            int employeeId = Convert.ToInt32(req.Query["employeeId"]);

            String firstName = req.Query["firstName"];

            String lastName = req.Query["lastName"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            firstName = firstName ?? data?.firstName;
            lastName = lastName ?? data?.lastName;
            string responseMessage = string.IsNullOrEmpty(firstName)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {firstName} {lastName} your employee ID is: {employeeId}. This HTTP triggered function executed successfully.";

            OkObjectResult okObjectResult = new OkObjectResult(responseMessage);
            return okObjectResult;
        }
    }
    public static class Sum
    {
        [FunctionName("Sum")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int? a = Convert.ToInt32(req.Query["number1"]);
            int? b = Convert.ToInt32(req.Query["number2"]);
            int result = a.GetValueOrDefault() + b.GetValueOrDefault();

            OkObjectResult okObjectResult = new OkObjectResult(result);
            return okObjectResult;
        }
    } 
}
        
