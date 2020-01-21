using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using PulseSnakeScoreAPI.Models;

namespace PulseSnakeScoreAPI
{
    public static class getAllScores
    {
        [FunctionName("getAllScores")] //HTTP Trigger that returns all the scores
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getAllScores")] HttpRequest req,
            ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("connectionstringdb");
            List<ScoreObject> scorelist = new List<ScoreObject>();
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM tblScores";

                        var result = await command.ExecuteReaderAsync();

                        while (await result.ReadAsync())
                        {
                            ScoreObject score = new ScoreObject()
                            {
                                ScoreId = Guid.Parse(result["ScoreId"].ToString()),
                                Name = result["Name"].ToString(),
                                Date = DateTime.Parse(result["Date"].ToString()),
                                Score = double.Parse(result["Score"].ToString()),
                                ScoreType = result["ScoreType"].ToString(),
                                Minuten = Int16.Parse(result["Minuten"].ToString()),
                            };
                            scorelist.Add(score);
                        }
                    }
                }
                return new OkObjectResult(scorelist);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "getAllScores");
                return new StatusCodeResult(500);
            }
        }
    }
}
