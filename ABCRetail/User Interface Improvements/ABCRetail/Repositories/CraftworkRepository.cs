using Microsoft.EntityFrameworkCore;

namespace ABCRetail.Repositories
{
    public interface ICraftworkRepository
    {
        Task AddProduct(Product product);
        Task DeleteProduct(Product product);
        Task<Product?> GetProductById(int id);
        Task<IEnumerable<Product>> GetProducts();
        Task UpdateProduct(Product product);
    }

    public class CraftworkRepository : ICraftworkRepository
    {
        private readonly ApplicationDbContext _context;
        public CraftworkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _context.Product.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(Product product)
        {
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product?> GetProductById(int id) => await _context.Product.FindAsync(id);

        public async Task<IEnumerable<Product>> GetProducts() => await _context.Product.Include(a=>a.Category).ToListAsync();
    }
}
