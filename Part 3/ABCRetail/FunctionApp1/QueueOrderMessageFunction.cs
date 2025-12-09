using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public static class QueueOrderMessageFunction
    {
        [Function("QueueOrderMessage")]
        public static async Task<IActionResult> Run(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string message = data?.Message;
            if (string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult("Invalid message data.");
            }

            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string queueName = "orders-queue"; // Update your queue name here
            QueueClient queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);

            log.LogInformation($"Message queued successfully: {message}");
            return new OkObjectResult("Message enqueued.");
        }
    }
}