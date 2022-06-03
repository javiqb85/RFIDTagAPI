using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;

namespace RFIDTagAPI
{
    public static class Authentication
    {
        [FunctionName("Authentication")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                string _RFIDvalueStr = req.Query["RFID"];
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                _RFIDvalueStr = _RFIDvalueStr ?? data?.RFID;
                if (!string.IsNullOrEmpty(_RFIDvalueStr))
                {
                    TableClient tableClient = new TableClient(Shared.AzureStorageConnectionString, Shared.AzureTablename);
                    var resptblCreate = tableClient.CreateIfNotExists();
                    Azure.Pageable<RFIDEntity> querytbl = tableClient.Query<RFIDEntity>(e => e.PartitionKey == Shared.PartitionKey && e.RowKey == _RFIDvalueStr);
                    foreach (var entry in querytbl)
                    {
                        return new OkObjectResult("true");
                    }
                    return new OkObjectResult("false");
                }
                else
                {
                    return new BadRequestObjectResult("RFID tag value as query parameter is required! ");
                }
            }
            catch (Exception ex)
            {
                log.LogInformation("Endpoint [Authentication] misbehaved with error:" + ex.Message);//logging for internal 
                return new StatusCodeResult(500);// internal server error

            }
        }
    }
}
