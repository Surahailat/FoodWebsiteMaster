using System.Net.Mail;
using System.Net;
using FoodWebsiteMaster.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FoodWebsiteMaster.Models.viewModel;
using System.Text.Json;

namespace FoodWebsiteMaster.Controllers
{
    public class userController : Controller
    {
        private readonly MyDbContext _context;

        public userController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult signIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> signIn(string email, string password)
        {

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                ViewBag.Message = "Wrong email or password!";
                return View();
            }
            else if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Phone", user.Phone);
                HttpContext.Session.SetString("Email", user.Email);
                _context.SaveChanges();

                // نقل بيانات سلة التسوق من الكوكيز إلى قاعدة البيانات
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
                            // التأكد من وجود كارت للمستخدم أو إنشائه إذا لم يكن موجودًا
                            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);
                            if (cart == null)
                            {
                                cart = new Cart { UserId = user.Id, CreatedAt = DateTime.Now };
                                _context.Carts.Add(cart);
                                await _context.SaveChangesAsync();
                            }

                            foreach (var cookieItem in cartListFromCookie)
                            {
                                // التحقق إذا كان المنتج موجودًا بالفعل في سلة التسوق
                                var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(
                                    item => item.CartId == cart.Id && item.ProductId == cookieItem.ProductID);

                                if (existingCartItem != null)
                                {
                                    // زيادة الكمية إذا كان المنتج موجودًا
                                    existingCartItem.Quantity += cookieItem.Quantity;
                                    existingCartItem.UpdatedAt = DateTime.Now;
                                    _context.Update(existingCartItem);
                                }
                                else
                                {
                                    // إضافة عنصر جديد إلى سلة التسوق
                                    var newCartItem = new CartItem
                                    {
                                        CartId = cart.Id,
                                        ProductId = cookieItem.ProductID,
                                        Quantity = cookieItem.Quantity,
                                        AddedAt = cookieItem.AddedAt, // الاحتفاظ بتاريخ الإنشاء الأصلي من الكوكي
                                        UpdatedAt = DateTime.Now,
                                        UserId = user.Id // يجب تعيين UserId هنا أيضًا
                                    };
                                    _context.CartItems.Add(newCartItem);
                                }
                            }
                            await _context.SaveChangesAsync();

                            // مسح الكوكي بعد نقل البيانات
                            Response.Cookies.Delete(cartCookie);
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Error deserializing cart cookie during login: {ex.Message}");
                        // يمكنك هنا إضافة منطق للتعامل مع خطأ فك تسلسل الكوكي، مثل مسحه
                        Response.Cookies.Delete(cartCookie);
                    }
                }
            }

            return RedirectToAction("Home2","Main");
        }
        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Phone", user.Phone);
                HttpContext.Session.SetString("Email", user.Email);

                return RedirectToAction("signIn");

            }
            return View(user);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("signIn");
        }
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
