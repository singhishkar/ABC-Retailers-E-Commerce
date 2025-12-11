using ABCRetail.Models;
using ABCRetail.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ABCRetail.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductRepository _productRepository;

        public ProductsController(ILogger<ProductsController> logger, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sterm = "", int categoryId = 0)
        {
            _logger.LogInformation("Accessing the Index action with search term '{SearchTerm}' and category ID {CategoryId}", sterm, categoryId);
            try
            {
                var products = await _productRepository.GetProducts(sterm, categoryId) ?? new List<Product>();
                var categories = await _productRepository.Categories() ?? new List<Category>();

                var productModel = new ProductDisplayModel
                {
                    Products = products,
                    Categories = categories,
                    STerm = sterm,
                    CategoryId = categoryId
                };

                return View(productModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while accessing the Index action.");
                return View("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
