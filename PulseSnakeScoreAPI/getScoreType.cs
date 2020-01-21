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
using PulseSnakeScoreAPI.Models;
using System.Data.SqlClient;

namespace PulseSnakeScoreAPI
{
    public static class getScoreType
    {
        [FunctionName("getScoreType")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getScoreType/{scoreType}")] HttpRequest req, string scoreType, 
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
                        if (scoreType == "Score") {
                            command.CommandText = "SELECT top 5 * , (Score/Minuten) as 'ScorePerMinuut' FROM tblScores where ScoreType = @scoreType order by 'ScorePerMinuut' desc;";
                        }
                        else
                        {
                            command.CommandText = "SELECT top 5 * FROM tblScores where ScoreType = @scoreType order by 'Score' desc;";
                        }
                        command.Parameters.AddWithValue("@scoreType", scoreType);

                        var result = await command.ExecuteReaderAsync();

                        while (await result.ReadAsync())
                        {
                            log.LogInformation(result.GetType().ToString());
                            ScoreObject score = new ScoreObject()
                            {
                                ScoreId = Guid.Parse(result["ScoreId"].ToString()),
                                Name = result["Name"].ToString(),
                                Date = DateTime.Parse(result["Date"].ToString()),
                                Score = double.Parse(result["Score"].ToString()),
                                ScoreType = result["ScoreType"].ToString(),
                                Minuten = Int16.Parse(result["Minuten"].ToString()),
                            };
                            if (scoreType == "Score")
                            {
                                score.ScorePerMinuut = double.Parse(result["ScorePerMinuut"].ToString());
                            }
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
