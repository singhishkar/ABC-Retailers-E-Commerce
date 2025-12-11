using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ABCRetail.Models;
using ABCRetail.Services;
using System.Security.Claims;
using Azure;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace ProductsMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userTableService;
        private readonly HttpClient _httpClient;

        public UsersController(UserService userService, HttpClient httpClient)
        {
            _userTableService = userService;
            _httpClient = httpClient;       
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _userTableService.GetAllCustomersAsync();
            return View(customers);
        }

        public async Task<IActionResult> Details(string id)
        {
            var customer = await _userTableService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustEmail,CustPasswordHash")] User customer)
        {
            if (ModelState.IsValid)
            {
                customer.PartitionKey = customer.CustEmail;
                customer.RowKey = Guid.NewGuid().ToString();

                bool result = await _userTableService.AddCustomerAsync(customer);

                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.ErrorMessage = "Failed to create customer.";
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var customer = await _userTableService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustEmail,CustPassword,CustPasswordHash,RowKey,PartitionKey")] User customer)
        {
            if (id != customer.RowKey)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(customer.CustPassword))
                    {
                        customer.CustPasswordHash = HashPassword(customer.CustPassword);
                    }
                    else
                    {
                        var existingCustomer = await _userTableService.GetCustomerAsync(customer.RowKey);
                        if (existingCustomer != null)
                        {
                            customer.CustPasswordHash = existingCustomer.CustPasswordHash;
                        }
                    }

                    await _userTableService.UpdateCustomerAsync(customer);
                    return RedirectToAction(nameof(Index));
                }
                catch (RequestFailedException)
                {
                    throw;
                }
            }

            return View(customer);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var customer = await _userTableService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _userTableService.GetCustomerAsync(id);
            if (customer != null)
            {
                await _userTableService.DeleteCustomerAsync(customer.PartitionKey, id);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid input.";
                return View();
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Email and password are required.";
                return View();
            }

            var customer = await _userTableService.FindCustomerByEmailAsync(email);
            if (customer != null)
            {
                var hashedInputPassword = HashPassword(password);

                if (customer.CustPasswordHash == hashedInputPassword)
                {
                    // Set session values
                    HttpContext.Session.SetString("CustId", customer.RowKey); // Changed to CustId for consistency
                    HttpContext.Session.SetString("CustomerEmail", customer.CustEmail);
                    HttpContext.Session.SetString("LOGGEDIN", "true");

                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid login attempt. Please check your credentials.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid login attempt. Please check your credentials.";
            }

            return View();
        }

     
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", string.Empty);
            }
        }
        /* [HttpPost]
         public async Task<IActionResult> SignUp(string email, string password)
         {
             if (ModelState.IsValid)
             {
                 var hashedPassword = HashPassword(password);

                 var customer = new User
                 {
                     CustEmail = email,
                     CustPassword = password,
                     CustPasswordHash = hashedPassword,
                     PartitionKey = email,
                     RowKey = email
                 };

                 bool result = await _userTableService.AddCustomerAsync(customer);

                 if (result)
                 {
                     return RedirectToAction("Login");
                 }
                 else
                 {
                     ViewBag.ErrorMessage = "A user with this email already exists.";
                     return View(customer);
                 }
             }

             return View();
         }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(string email, string password)
        {
            Console.WriteLine("SignUp method called");
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                Console.WriteLine($"Model is valid. Email: {email}");
                var jsonContent = JsonSerializer.Serialize(new { email, password });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://ishkfunction.azurewebsites.net/api/SignUp", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                if (result != null && result.ContainsKey("success") && result["success"].GetBoolean())
                {
                    Console.WriteLine("Customer successfully added.");
                    return RedirectToAction("Login");
                }
                else
                {
                    var message = result.ContainsKey("message") ? result["message"].GetString() : "An error occurred.";
                    Console.WriteLine("Failed to add customer: " + message);
                    ViewBag.ErrorMessage = message;
                    return View();
                }
            }
            else
            {
                Console.WriteLine("Model state is not valid.");
                ViewBag.ErrorMessage = "Both Email and Password are required.";
                return View();
            }
        }



        [HttpPost]
        
        public IActionResult Logout()
        {
            // Clear the session variable that tracks login status
            HttpContext.Session.SetString("LOGGEDIN", "false");

            // Optionally, you can also clear all session data
            HttpContext.Session.Clear();

            // Redirect to the home page or login page
            return RedirectToAction("Index", "Home");
        }
    }
}
