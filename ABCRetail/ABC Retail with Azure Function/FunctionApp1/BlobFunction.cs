using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
        public class BlobFunction
        {
            private readonly BlobServiceClient _blobServiceClient;
            private readonly string _containerName = "products";  // Container name

            public BlobFunction(BlobServiceClient blobServiceClient)
            {
                _blobServiceClient = blobServiceClient;
            }

            // Upload Blob Function
            [Function("UploadBlob")]
            public async Task<IActionResult> UploadBlobAsync(
                [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")] HttpRequest req,
                ILogger log)
            {
                log.LogInformation("C# HTTP trigger function to upload a blob.");

                // Get the file from the request body
                var file = req.Form.Files["file"];
                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("File is missing.");
                }

                // Get a reference to the container and blob client
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(file.FileName);

                // Upload the file to blob storage
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                // Return the URI of the uploaded blob
                return new OkObjectResult(new { uri = blobClient.Uri.ToString() });
            }

            // Delete Blob Function
            //[FunctionName("DeleteBlob")]
            //public async Task<IActionResult> DeleteBlobAsync(
            //    [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Function, "delete", Route = "delete")] HttpRequest req,
            //    ILogger log)
            //{
            //    log.LogInformation("C# HTTP trigger function to delete a blob.");

            //    // Get the blob URI from the query string or body
            //    string blobUri = req.Query["blobUri"];
            //    if (string.IsNullOrEmpty(blobUri))
            //    {
            //        return new BadRequestObjectResult("Blob URI is missing.");
            //    }

            //    Uri uri = new Uri(blobUri);
            //    string blobName = uri.Segments[^1];

            //    var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            //    var blobClient = containerClient.GetBlobClient(blobName);

            //    await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);

            //    return new OkResult();
            //}
        }
    }