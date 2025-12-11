using Microsoft.EntityFrameworkCore;

namespace ABCRetail.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> Categories()
        {
            return await _db.Category.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();
            var products = await (from product in _db.Product
                                  join category in _db.Category
                                  on product.CategoryId equals category.Id
                                  join stock in _db.Stocks
                                  on product.Id equals stock.ProductId
                                  into product_stocks
                                  from productWithStock in product_stocks.DefaultIfEmpty()
                                  where string.IsNullOrWhiteSpace(sTerm) || (product != null && product.ProductName.ToLower().StartsWith(sTerm))
                                  select new Product
                                  {
                                      Id = product.Id,
                                      Image = product.Image,
                                      Description = product.Description,
                                      ProductName = product.ProductName,
                                      CategoryId = product.CategoryId,
                                      Price = product.Price,
                                      CategoryName = category.CategoryName, // Ensure this is correct
                                      Quantity = productWithStock == null ? 0 : productWithStock.Quantity
                                  }).ToListAsync();

            if (categoryId > 0)
            {
                products = products.Where(a => a.CategoryId == categoryId).ToList();
            }
            return products;
        }
    }
}
