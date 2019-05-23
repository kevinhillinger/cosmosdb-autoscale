using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.CosmosDb.Autoscale
{
    public static class Handlers
    {
        [FunctionName("evaluateRequestUnitsHandler")]
        public static async Task<IActionResult> EvaluateRequestUnitsHandler(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation("C# HTTP trigger function processed a request.");
            
            var alert = GetRequestUnitAlert(
                await new StreamReader(req.Body).ReadToEndAsync()
                );

            using (client = GetCosmosClient(config))
            {
                var offer = client.CreateOfferQuery().Where(r => r.ResourceLink == collection.SelfLink).Single();
     
                var offer = new OfferV2(offer, newthroughput);
                client.ReplaceOfferAsync(offer); 
            }
  
            return name != null
                ? (ActionResult)new OkObjectResult($"Parsed")
                : new BadRequestObjectResult("Bad request.");
        }

        private static RequestUnitAlert GetRequestUnitAlert(string requestBody) 
        {
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            return CommonAlertParser.Parse(data);
        }

        private static DocumentClient GetCosmosClient(Configuration config) 
        {
            var connectionPolicy = new ConnectionPolicy { UserAgentSuffix = " cosmosdb/reactive" };
            var client = new DocumentClient(new Uri(config["cosmosdbEndpoint"]), config["authorizationKey"], connectionPolicy);

            return client;
        }
    }
}
