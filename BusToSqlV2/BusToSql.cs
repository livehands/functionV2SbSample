using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;

namespace BusToSqlV2
{
    public static class BusToSql
    {
        [FunctionName("BusToSql2")] 
        public static void Run([ServiceBusTrigger("ionmessages", Connection = "sbConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            var or = JsonConvert.DeserializeObject<ObjectResult>(myQueueItem);

            //Extract the Value from the ObjectResult sent from the HTTP method and use the Value.
            MyMessage e = or.Value as MyMessage;
            log.LogInformation($"Message: {e.Id}, {e.FirstName}, {e.LastName}");

            ///Write to SQL
            var cnnString = Environment.GetEnvironmentVariable("sqlConnection");

            using (SqlConnection conn = new SqlConnection(cnnString))
            {
                conn.Open();

                // Insert Signup        
                var signupInsert = "INSERT INTO [dbo].[Messages] ([FirstName],[LastName],[Id])" +
                "VALUES ('" + e.FirstName + "','" + e.LastName + "','" + e.Id + "')";

                log.LogInformation($"Insert: {signupInsert}");

                // Execute and load data into database.
                using (SqlCommand cmd = new SqlCommand(signupInsert, conn))
                {
                    var rows = cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
