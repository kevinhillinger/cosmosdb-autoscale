#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

// Install-Package Microsoft.Azure.DocumentDB.Core -Version 2.4.0
public  static DocumentClient client = new DocumentClient(new Uri(endpointUrl), authorizationKey);

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("Scale up RUs on Cosmos DB");

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);

    log.LogInformation($"{data}");

    var offer = client.CreateOfferQuery()
            .Where(r => r.ResourceLink == collection.SelfLink).Single();
        offer = new OfferV2(offer, newthroughput);
        client.ReplaceOfferAsync(offer);
    
    return (ActionResult)new OkObjectResult($"{data}");
}
