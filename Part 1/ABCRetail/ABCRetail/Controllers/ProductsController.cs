
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ABCRetail.Models;
using ABCRetail.Services;

namespace Cloud_Storage.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;

        public ProductsController(BlobService blobService, TableStorageService tableStorageService, QueueService queueService)
        {
            _blobService = blobService;
            _tableStorageService = tableStorageService;
            _queueService = queueService;
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            if (HttpContext.Session.GetString("LOGGEDIN") != "true")
            {
                return RedirectToAction("Login", "Users");
            }

            var product = new Product(); // Initialize the Product model
            return View(product); // Pass the model to the view
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile file)
        {
            if (HttpContext.Session.GetString("LOGGEDIN") != "true")
            {
                return RedirectToAction("Login", "Users");
            }

            if (file != null)
            {
                using var stream = file.OpenReadStream();
                var imageUrl = await _blobService.UploadAsync(stream, file.FileName);
                product.ImageUrl = imageUrl; // Save the image URL in the product
            }

            if (ModelState.IsValid)
            {
                product.PartitionKey = "ProductsPartition";
                product.RowKey = Guid.NewGuid().ToString(); // Generate a unique ID for the product
                await _tableStorageService.AddProductAsync(product); // Add the product to Azure Table Storage
                return RedirectToAction("Index");
            }

            return View(product); // If the model is invalid, return the view with the current product model
        }


        [HttpGet]
        public async Task<IActionResult> Cart(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return BadRequest("Product ID is required.");
            }

            var product = await _tableStorageService.GetProductAsync("ProductsPartition", productId);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new CartViewModel
            {
                Product = product,
                Quantity = 1
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(CartViewModel model)
        {
            // Handle the order logic
            var userEmail = HttpContext.Session.GetString("CustomerEmail");
            var message = $"{userEmail} ordered {model.Product.ProductName} (Quantity: {model.Quantity})";

            // Send a message to the queue
            await _queueService.SendMessageAsync(message);

            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var userEmail = HttpContext.Session.GetString("CustomerEmail");

            if (userEmail != "Admin@gmail.com")
            {
                return Unauthorized(); // Redirect to unauthorized access page or home
            }

            var messages = await _queueService.GetMessagesAsync();
            return View(messages);
        }



        [HttpGet]
        public async Task<IActionResult> EditProduct(string partitionKey, string rowKey)
        {
            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                var imageUrl = await _blobService.UploadAsync(stream, file.FileName);
                product.ImageUrl = imageUrl;
            }

            if (ModelState.IsValid)
            {
                await _tableStorageService.UpdateProductAsync(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string partitionKey, string rowKey)
        {
            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            if (product != null && !string.IsNullOrEmpty(product.ImageUrl))
            {
                await _blobService.DeleteBlobAsync(product.ImageUrl);
            }
            await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("LOGGEDIN") != "true")
            {
                return RedirectToAction("Login", "Users");
            }

            var products = await _tableStorageService.GetAllProductsAsync();

            // Check if the user is an admin
            if (HttpContext.Session.GetString("CustomerEmail") == "Admin@gmail.com")
            {
                return View("AdminIndex", products); // Admin view with edit/delete options
            }
            return View("Index", products); // Regular view without edit/delete options
        }
    }
}
