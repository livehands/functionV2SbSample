using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BusToSqlV2
{
    public static class HttpToBus
    {
        [FunctionName("HttpToBus")]
        [return: ServiceBus("ionmessages", Connection = "sbConnection")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            MyMessage m = new MyMessage()
            {
                FirstName = "IonV2",
                LastName = DateTime.Now.ToShortDateString(),
                Id = DateTime.Now.Minute
            };

            string body = JsonConvert.SerializeObject(m);

            log.LogInformation($"V2 Message Body: {body}");

            return new OkObjectResult(body);
        }
    }
}
