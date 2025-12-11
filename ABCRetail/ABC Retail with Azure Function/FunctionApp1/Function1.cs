using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ABCRetail.Models;
using System.Security.Cryptography;
using System.Text;
using Azure.Data.Tables;

namespace FunctionApp1
{
    public class Function1
    {
        [Function("SignUp")]
        public static async Task<IActionResult> Runn(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log,
        ExecutionContext context)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string email = data?.email;
            string password = data?.password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return new BadRequestObjectResult(new { success = false, message = "Email and password are required." });
            }

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var tableClient = new TableServiceClient(connectionString).GetTableClient("Users");

            await tableClient.CreateIfNotExistsAsync();

            var existingEntity = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Customers' and RowKey eq '{email}'").FirstOrDefault();
            if (existingEntity != null)
            {
                return new BadRequestObjectResult(new { success = false, message = "A user with this email already exists." });
            }

            var hashedPassword = HashPassword(password);
            var entity = new TableEntity
            {
                ["PartitionKey"] = email,
                ["RowKey"] = Guid.NewGuid().ToString(),
                ["CustEmail"] = email,
                ["CustPassword"] = password,  // Consider security implications
                ["CustPasswordHash"] = hashedPassword,
               ["CustId"] = 0,

            };

            try
            {
                await tableClient.AddEntityAsync(entity);
                return new OkObjectResult(new { success = true, message = "Customer registered successfully." });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { success = false, message = "Error registering customer." });
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", string.Empty);
            }
        }

    }
}
