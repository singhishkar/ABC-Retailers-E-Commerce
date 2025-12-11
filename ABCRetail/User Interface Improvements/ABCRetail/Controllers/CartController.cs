using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }
        public async Task<IActionResult> AddItem(int productId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(productId, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int productId)
        {
            var cartCount = await _cartRepo.RemoveItem(productId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        public  async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }

        public  IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            // Log the start of the checkout process
            Console.WriteLine("Starting Checkout Process...");

            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid. Listing errors:");

                // Iterate through all model state entries and log their errors
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        // Log each error in the ModelState entry
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"Property: {state.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                }

                // Return the view with the model to display errors
                return View(model);
            }

            // Proceed with checkout if model state is valid
            bool isCheckedOut = await _cartRepo.DoCheckout(model);
            if (!isCheckedOut)
            {
                Console.WriteLine("Checkout failed, redirecting to OrderFailure.");
                return RedirectToAction(nameof(OrderFailure));
            }

            Console.WriteLine("Checkout successful, redirecting to OrderSuccess.");
            return RedirectToAction(nameof(OrderSuccess));
        }


        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }

    }
}
