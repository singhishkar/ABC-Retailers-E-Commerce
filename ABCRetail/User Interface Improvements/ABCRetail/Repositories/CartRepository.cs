using ABCRetail.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ABCRetail.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int productId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ProductId == productId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var product = _db.Product.Find(productId);
                    cartItem = new CartDetail
                    {
                        ProductId = productId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = product.Price  // it is a new line after update
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }


        public async Task<int> RemoveItem(int productId)
        {
            //using var transaction = _db.Database.BeginTransaction();
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new InvalidOperationException("Invalid cart");
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ProductId == productId);
                if (cartItem is null)
                    throw new InvalidOperationException("Not items in cart");
                else if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("Invalid userid");
            var shoppingCart = await _db.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Product)
                                  .ThenInclude(a => a.Stock)
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Product)
                                  .ThenInclude(a => a.Category)
                                  .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;

        }
        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId)) // updated line
            {
                userId = GetUserId();
            }
            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              where cart.UserId == userId // updated line
                              select new { cartDetail.Id }
                        ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                Console.WriteLine("Starting checkout process...");

                // Fetch the user ID and validate
                var userId = GetUserId();
                Console.WriteLine($"User ID: {userId}");
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Checkout failed: User is not logged-in");
                    throw new UnauthorizedAccessException("User is not logged-in");
                }

                // Fetch the cart and validate
                var cart = await GetCart(userId);
                Console.WriteLine($"Cart: {cart?.Id}");
                if (cart == null)
                {
                    Console.WriteLine("Checkout failed: Invalid cart");
                    throw new InvalidOperationException("Invalid cart");
                }

                // Fetch the cart details and validate
                var cartDetail = _db.CartDetails.Where(a => a.ShoppingCartId == cart.Id).ToList();
                Console.WriteLine($"Cart details count: {cartDetail.Count}");
                if (cartDetail.Count == 0)
                {
                    Console.WriteLine("Checkout failed: Cart is empty");
                    throw new InvalidOperationException("Cart is empty");
                }

                // Check for pending order status
                var pendingRecord = _db.orderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                Console.WriteLine($"Order Status: {pendingRecord?.StatusName}");
                if (pendingRecord == null)
                {
                    Console.WriteLine("Checkout failed: Order status does not have Pending status");
                    throw new InvalidOperationException("Order status does not have Pending status");
                }

                // Create order
                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id
                   
                };
                _db.Orders.Add(order);
                _db.SaveChanges();

                foreach (var item in cartDetail)
                {
                    // Add order details and update stock
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                    Console.WriteLine($"Added order detail for ProductId {item.ProductId}");

                    var stock = await _db.Stocks.FirstOrDefaultAsync(a => a.ProductId == item.ProductId);
                    if (stock == null)
                    {
                        Console.WriteLine("Checkout failed: Stock is null");
                        throw new InvalidOperationException("Stock is null");
                    }

                    if (item.Quantity > stock.Quantity)
                    {
                        Console.WriteLine($"Checkout failed: Only {stock.Quantity} item(s) are available in the stock");
                        throw new InvalidOperationException($"Only {stock.Quantity} items(s) are available in the stock");
                    }

                    stock.Quantity -= item.Quantity;
                    Console.WriteLine($"Updated stock for ProductId {item.ProductId}, new quantity: {stock.Quantity}");
                }

                _db.SaveChanges();

                // Remove the cart details
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                Console.WriteLine("Checkout completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Checkout failed: {ex.Message}");
                return false;
            }
        }


        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }


    }
}
