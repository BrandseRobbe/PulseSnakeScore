using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PulseSnakeScoreAPI.Models;
using System.Data.SqlClient;

namespace PulseSnakeScoreAPI
{
    public static class newScore
    {
        [FunctionName("newScore")] //HTTP Trigger that iserts a score into the db
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "newScore")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string connectionstring = Environment.GetEnvironmentVariable("connectionstringdb");
                string json = await new StreamReader(req.Body).ReadToEndAsync();

                ScoreObject newScore= JsonConvert.DeserializeObject<ScoreObject>(json);
                newScore.ScoreId = Guid.NewGuid();
                newScore.Date = DateTime.Now;

                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionstring;
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = "INSERT INTO tblScores (ScoreId, Name, Date, ScoreType, Score, Minuten) VALUES (@ScoreId, @Name, @Date, @ScoreType, @Score, @Minuten)";
                        command.Parameters.AddWithValue("@ScoreId", newScore.ScoreId);
                        command.Parameters.AddWithValue("@Name", newScore.Name);
                        command.Parameters.AddWithValue("@Date", newScore.Date);
                        command.Parameters.AddWithValue("@ScoreType", newScore.ScoreType);
                        command.Parameters.AddWithValue("@Score", newScore.Score);
                        command.Parameters.AddWithValue("@Minuten", newScore.Minuten);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return new OkObjectResult(newScore);
            }
            catch (Exception ex)
            {

                log.LogError(ex, "newScore");
                return new StatusCodeResult(500);
            }
        }
    }
}
