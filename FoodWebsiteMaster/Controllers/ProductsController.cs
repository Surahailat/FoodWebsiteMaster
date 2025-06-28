using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodWebsiteMaster.Models;
using FoodWebsiteMaster.Models.viewModel;
using System.Text.Json;

namespace FoodWebsiteMaster.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyDbContext _context;

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Shop(string sortOrder, string searchString)
        {
            var products = from p in _context.Products select p;

            // Filter by search string
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "az":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "za":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "price":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    break; // No sorting (default)
            }

            return View(await products.ToListAsync());
        }


        // GET: Products/Details/5
        public async Task<IActionResult> singleProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,StockQuantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,StockQuantity")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Cart()
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            List<CartItem> cartItems;

            if (sessionUserId != null)
            {
                // ✅ إذا كان المستخدم مسجل دخول
                int userId = sessionUserId.Value;
                cartItems = await _context.CartItems
                    .Where(ci => ci.UserId == userId)
                    .Include(ci => ci.Product) // مهم لعرض اسم المنتج وصورته
                    .ToListAsync();
            }
            else
            {
                // ✅ إذا كان المستخدم غير مسجل دخول (زائر)
                const string cartCookieName = "temporaryCart";
                string cartCookieValue = Request.Cookies[cartCookieName];
                List<tempCart> cartList = new();

                if (!string.IsNullOrEmpty(cartCookieValue))
                {
                    try
                    {
                        cartList = JsonSerializer.Deserialize<List<tempCart>>(cartCookieValue) ?? new();
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Error reading cookie: {ex.Message}");
                    }
                }

                // تحويل قائمة المنتجات في الكوكيز إلى CartItem
                cartItems = cartList.Select(c =>
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == c.ProductID);
                    return new CartItem
                    {
                        ProductId = c.ProductID,
                        Quantity = c.Quantity,
                        Product = product,
                        Price = c.Price,         // ⭐️ السعر من الكوكي
                        Image = c.Image          // ⭐️ الصورة من الكوكي
                    };
                }).ToList();

            }

            return View(cartItems);
        }


        //add to cart 
        [Route("Products/AddToCart/{id}")]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            int? sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId != null)
            {
                // مستخدم مسجل دخول
                var userId = sessionUserId.Value;

                // ابحث عن كارت مرتبط بهذا اليوزر
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cart == null)
                {
                    // إذا ما عنده كارت، أنشئ كارت جديد
                    cart = new Cart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // ابحث عن منتج موجود بالكارت
                var existingCartItem = await _context.CartItems
                    .FirstOrDefaultAsync(item => item.CartId == cart.Id && item.ProductId == id);

                if (existingCartItem != null)
                {
                    // إذا المنتج موجود، زيد الكمية
                    existingCartItem.Quantity++;
                    existingCartItem.UpdatedAt = DateTime.Now;
                    _context.CartItems.Update(existingCartItem);
                }
                else
                {
                    // إذا مش موجود، ضيفه جديد
                    var newCartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = id,
                        Quantity = 1,
                        Price = product.Price,
                        AddedAt = DateTime.Now,
                        UserId = userId,
                        UpdatedAt = DateTime.Now,
                        Image = product.Image // اذا عندك صورة بالمنتج
                    };
                    _context.CartItems.Add(newCartItem);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                const string cartCookieName = "temporaryCart";
                string cartCookieValue = Request.Cookies[cartCookieName];
                List<tempCart> cartList = new List<tempCart>();

                if (!string.IsNullOrEmpty(cartCookieValue))
                {
                    try
                    {
                        cartList = JsonSerializer.Deserialize<List<tempCart>>(cartCookieValue);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Error deserializing cart cookie: {ex.Message}");
                        cartList = new List<tempCart>();
                    }
                }

                var existingItem = cartList.FirstOrDefault(item => item.ProductID == id);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cartList.Add(new tempCart
                    {
                        ProductID = id,
                        Quantity = 1,
                        AddedAt = DateTime.Now,
                        Image= product.Image,
                        Price = product.Price
                    });
                }

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(30)
                };

                Response.Cookies.Append(cartCookieName, JsonSerializer.Serialize(cartList), cookieOptions);
            }

            return RedirectToAction("Shop");
        }

        // لزيادة الكمية
        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                cartItem.UpdatedAt = DateTime.Now;
                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Cart");
        }

        // لتنقيص الكمية
        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    cartItem.UpdatedAt = DateTime.Now;
                    _context.CartItems.Update(cartItem);
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Cart");
        }

        // لحذف المنتج من السلة
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Cart");
        }




        public async Task<IActionResult> AddNumCart(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            int? sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId != null)
            {
                var userId = sessionUserId.Value;

                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cart == null)
                {
                    cart = new Cart { UserId = userId, CreatedAt = DateTime.Now };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var existingCartItem = await _context.CartItems
                    .FirstOrDefaultAsync(item => item.CartId == cart.Id && item.ProductId == id);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                    existingCartItem.UpdatedAt = DateTime.Now;
                    _context.CartItems.Update(existingCartItem);
                }
                else
                {
                    var newCartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = id,
                        Quantity = quantity,
                        Price = product.Price,
                        AddedAt = DateTime.Now,
                        UserId = userId,
                        UpdatedAt = DateTime.Now,
                        Image = product.Image
                    };
                    _context.CartItems.Add(newCartItem);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                const string cartCookieName = "temporaryCart";
                string cartCookieValue = Request.Cookies[cartCookieName];
                List<tempCart> cartList = new List<tempCart>();

                if (!string.IsNullOrEmpty(cartCookieValue))
                {
                    try { cartList = JsonSerializer.Deserialize<List<tempCart>>(cartCookieValue); }
                    catch { cartList = new List<tempCart>(); }
                }

                var existingItem = cartList.FirstOrDefault(item => item.ProductID == id);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cartList.Add(new tempCart
                    {
                        ProductID = id,
                        Quantity = quantity,
                        AddedAt = DateTime.Now
                    });
                }

                var cookieOptions = new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) };
                Response.Cookies.Append(cartCookieName, JsonSerializer.Serialize(cartList), cookieOptions);
            }

            return RedirectToAction("Shop");
        }



    }
}
