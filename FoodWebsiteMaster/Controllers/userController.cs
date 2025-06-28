using System.Net.Mail;
using System.Net;
using FoodWebsiteMaster.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FoodWebsiteMaster.Models.viewModel;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;


namespace FoodWebsiteMaster.Controllers
{
    public class userController : Controller
    {
        private readonly MyDbContext _context;

        public userController(MyDbContext context)
        {
            _context = context;
        }
        // /////////////////////////////////////////////////////////////////////
        // signIn
        public IActionResult signIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> signIn(string email, string password)
        {
            // Basic input validation (ensure email and password are not empty)
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and Password are required.");
                return View();
            }

            // Check if the user exists in the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // User not found - add a model error for email
                ModelState.AddModelError("Email", "Email not found!");
                return View();
            }

            // Check if the password is correct
            if (user.Password != password)
            {
                // Incorrect password - add a model error for password
                ModelState.AddModelError("Password", "Incorrect password!");
                return View();
            }

            // If the user exists and the password is correct
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Phone", user.Phone);
            HttpContext.Session.SetString("Email", user.Email);
            _context.SaveChanges();

            // Handle cart cookies (temporary cart for the user)
            const string cartCookie = "temporaryCart";
            string cartCookieValue = Request.Cookies[cartCookie];

            if (!string.IsNullOrEmpty(cartCookieValue))
            {
                List<tempCart> cartListFromCookie = new List<tempCart>();
                try
                {
                    cartListFromCookie = JsonSerializer.Deserialize<List<tempCart>>(cartCookieValue);

                    if (cartListFromCookie != null && cartListFromCookie.Any())
                    {
                        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);
                        if (cart == null)
                        {
                            cart = new Cart { UserId = user.Id, CreatedAt = DateTime.Now };
                            _context.Carts.Add(cart);
                            await _context.SaveChangesAsync();
                        }

                        foreach (var cookieItem in cartListFromCookie)
                        {
                            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(
                                item => item.CartId == cart.Id && item.ProductId == cookieItem.ProductID);

                            if (existingCartItem != null)
                            {
                                existingCartItem.Quantity += cookieItem.Quantity;
                                existingCartItem.UpdatedAt = DateTime.Now;
                                _context.Update(existingCartItem);
                            }
                            else
                            {
                                var newCartItem = new CartItem
                                {
                                    CartId = cart.Id,
                                    ProductId = cookieItem.ProductID,
                                    Quantity = cookieItem.Quantity,
                                    AddedAt = cookieItem.AddedAt,
                                    UpdatedAt = DateTime.Now,
                                    UserId = user.Id
                                };
                                _context.CartItems.Add(newCartItem);
                            }
                        }
                        await _context.SaveChangesAsync();
                        Response.Cookies.Delete(cartCookie);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing cart cookie during login: {ex.Message}");
                    Response.Cookies.Delete(cartCookie);
                }
            }

            // Redirect to the Home2 action in the Main controller after successful login
            return RedirectToAction("Home2", "Main");
        }


        // /////////////////////////////////////////////////////////////////////
        // Register

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                TempData["ErrorMessage"] = "Email is already registered.";
                return RedirectToAction("signIn"); 

            }

            if (_context.Users.Any(u => u.Phone == model.Phone))
            {
                TempData["ErrorMessage"] = "Phone number is already registered.";
                return RedirectToAction("signIn");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("signIn", model);

                //return View(model);
            }

            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                Username = model.Username,
                Phone = model.Phone,
                Email = model.Email,
            };

            user.Password = hasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Phone", user.Phone);
            HttpContext.Session.SetString("Email", user.Email);

            return RedirectToAction("signIn");
        }



        // /////////////////////////////////////////////////////////////////////
        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("signIn");
        }

        // /////////////////////////////////////////////////////////////////////
        // Profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("signIn", "user");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            return View(user); 
        }
        ///////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        public IActionResult UpdateProfile(User updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("signIn", "user");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                user.Username = updatedUser.Username;
                user.DateOfBirth = updatedUser.DateOfBirth;
                user.Gender = updatedUser.Gender;
                user.Height = updatedUser.Height;
                user.Weight = updatedUser.Weight;
                user.City = updatedUser.City;
                user.Country = updatedUser.Country;
                user.Phone = updatedUser.Phone;

                _context.SaveChanges();

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Phone", user.Phone);

                return RedirectToAction("profile");
            }

            return NotFound();
        }
        ////////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int userId = (int)HttpContext.Session.GetInt32("UserId");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View(model);
            }

            user.Password = model.NewPassword;
            _context.SaveChanges();

            ViewBag.Message = "Password updated successfully!";
            return View("Profile");
        }

    }
}
