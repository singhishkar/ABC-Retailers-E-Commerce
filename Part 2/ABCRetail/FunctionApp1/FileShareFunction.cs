using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class FileShareFunction
    {
        [Function("UploadFileFunction")]
        public static async Task<IActionResult> Run(
        [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string fileShareName = "product-share"; // Add the file share name

            try
            {
                // Get the directory name from form data
                string directoryName = req.Form["uploads"];
                IFormFile file = req.Form.Files["file"];

                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("No file uploaded.");
                }

                var serviceClient = new ShareServiceClient(connectionString);
                var shareClient = serviceClient.GetShareClient(fileShareName);
                await shareClient.CreateIfNotExistsAsync();

                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(file.FileName);

                using (var fileStream = file.OpenReadStream())
                {
                    await fileClient.CreateAsync(fileStream.Length);
                    await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length), fileStream);
                }

                return new OkObjectResult($"File '{file.FileName}' uploaded successfully.");
            }
            catch (Exception ex)
            {
                log.LogError($"Error uploading file: {ex.Message}");
                return new BadRequestObjectResult($"Error uploading file: {ex.Message}");
            }
        }
    }
}

