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
    public static class Create
    {
        [FunctionName("Create")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Create/RFID/{RFID_Value_str}")] HttpRequest req, string RFID_Value_str, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                if (!string.IsNullOrEmpty(RFID_Value_str))
                {
                    string _RFIDvalueStr = RFID_Value_str.Trim();
                    // log on to azure storage                     
                    TableClient tableClient = new TableClient(Shared.AzureStorageConnectionString, Shared.AzureTablename);
                    var resptblCreate = tableClient.CreateIfNotExists();
                    try
                    {
                        var existingRFID = await tableClient.GetEntityAsync<RFIDEntity>(Shared.PartitionKey, _RFIDvalueStr);
                    }
                    catch (Azure.RequestFailedException ex)
                    {
                        if (ex.Status == 404)// RFID does not exist
                        {
                            RFIDEntity rfidEntity = new RFIDEntity { PartitionKey = Shared.PartitionKey, RowKey = _RFIDvalueStr };
                            var resp = await tableClient.AddEntityAsync(rfidEntity);
                            if (!resp.IsError)
                            {
                                log.LogInformation($"RFID tag {_RFIDvalueStr} Created");
                                return new StatusCodeResult(StatusCodes.Status201Created);
                            }
                        }
                    }
                    return new ObjectResult(new { Result = $"RFID tag {_RFIDvalueStr} has already been added!" });

                }
                else
                {
                    return new BadRequestObjectResult("RFID tag value is required ");
                }
            }
            catch (Exception ex)
            {
                log.LogInformation("Endpoint [Create] misbehaved with error:" + ex.Message);//logging for internal 
                return new StatusCodeResult(500);// internal server error
                //Note! exposing orginal exception to audiance is not good idea for security stand point
                //In case we want to inform audiance with original exception we can build ContentResult as follows:

                //return new ContentResult() { ContentType = "text/html", Content=ex.Message, StatusCode=StatusCodes.Status500InternalServerError };
            }
        }
    }
}
