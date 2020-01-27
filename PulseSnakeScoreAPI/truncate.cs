using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;


namespace PulseSnakeScoreAPI
{
    public static class truncate
    {
        [FunctionName("truncate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reset")] HttpRequest req,
            ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("connectionstringdb");
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "truncate table tblScores";
                        var result = await command.ExecuteReaderAsync();
                    }
                }
                return new OkObjectResult("OK");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "getAllScores");
                return new StatusCodeResult(500);
            }
        }
    }
}
