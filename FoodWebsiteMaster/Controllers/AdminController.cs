using FoodWebsiteMaster.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodWebsiteMaster.Controllers
{
    public class AdminController : Controller
    {

            private readonly MyDbContext _context;

            public AdminController(MyDbContext context)
            {
                _context = context;
            }

        public IActionResult Index()
        {
            var userCount = _context.Users.Count();
            var ProductCount = _context.Products.Count();

            var currentDate = DateTime.Now;
            var subscribedUserCount = _context.UserSubscribes
                                               .Count(us => us.StartDate <= currentDate && us.EndDate >= currentDate);

            ViewBag.UserCount = userCount;
            ViewBag.SubscribedUserCount = subscribedUserCount;
            ViewBag.ProductCount = ProductCount;


            return View();
        }

        public async Task<IActionResult> user()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> UserSubscribe()
        {
            var usersSub = await _context.UserSubscribes.ToListAsync();
            return View(usersSub);
        }

        public async Task<IActionResult> Product()
        {
            var product = await _context.Products.ToListAsync();
            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Product"); 

            var existing = await _context.Products.FindAsync(updatedProduct.Id);
            if (existing == null)
                return NotFound();

            existing.Name = updatedProduct.Name;
            existing.Price = updatedProduct.Price;
            existing.StockQuantity = updatedProduct.StockQuantity;
            existing.Description = updatedProduct.Description;

            _context.Products.Update(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction("Product");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Product");
        }


        public IActionResult createProduct()
        {
            return View();

        }
        [HttpPost]
        public IActionResult createProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("Product");
            }

            return View(product);
        }

        public async Task<IActionResult> Doctor()
        {
            var Doctors = await _context.Doctors.ToListAsync();
            return View(Doctors);
        }
        [HttpPost]
        public async Task<IActionResult> EditDoctor(Doctor updatedDoctor)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Doctor"); 

            var update = await _context.Doctors.FindAsync(updatedDoctor.Id);
            if (update == null)
                return NotFound();

            update.Name = updatedDoctor.Name;
            update.ProfileImage = updatedDoctor.ProfileImage;
            update.Position = updatedDoctor.Position;
            update.InstagramLink = updatedDoctor.InstagramLink;

            _context.Doctors.Update(update);
            await _context.SaveChangesAsync();

            return RedirectToAction("Doctor");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var Doctor = await _context.Doctors.FindAsync(id);
            if (Doctor == null)
                return NotFound();

            _context.Doctors.Remove(Doctor);
            await _context.SaveChangesAsync();

            return RedirectToAction("Doctor");
        }

        public IActionResult AddDoctor()
        {
            return View();

        }
        [HttpPost]
        public IActionResult AddDoctor(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Doctors.Add(doctor);
                _context.SaveChanges();

                return RedirectToAction("Doctor");
            }

            return View(doctor);
        }


        public async Task<IActionResult> contact()
        {
            var contact = await _context.Contacts.ToListAsync();
            return View(contact);
        }

        public async Task<IActionResult> Recipe()
        {
            var recipes = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Directions)
                .ToListAsync();

            return View(recipes);
        }

        public async Task<IActionResult> EditRecipes(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Directions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRecipes(Recipe model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Directions)
                .FirstOrDefaultAsync(r => r.Id == model.Id);

            if (recipe == null) return NotFound();

            // تحديث بيانات الوصفة
            recipe.Name = model.Name;
            recipe.Type = model.Type;
            recipe.ServingSize = model.ServingSize;
            recipe.CaloriesTotal = model.CaloriesTotal;
            recipe.TotalFat = model.TotalFat;
            recipe.Carbohydrate = model.Carbohydrate;
            recipe.Protein = model.Protein;
            recipe.ImageUrl = model.ImageUrl;

            // تحديث المكونات
            foreach (var ing in model.Ingredients)
            {
                if (ing.Id == 0)
                {
                    // مكون جديد
                    recipe.Ingredients.Add(new Ingredient
                    {
                        IngredientText = ing.IngredientText,
                        RecipeId = recipe.Id
                    });
                }
                else
                {
                    // مكون موجود
                    var existingIng = recipe.Ingredients.FirstOrDefault(i => i.Id == ing.Id);
                    if (existingIng != null)
                    {
                        existingIng.IngredientText = ing.IngredientText;
                    }
                }
            }

            // تحديث التعليمات
            foreach (var dir in model.Directions)
            {
                if (dir.Id == 0)
                {
                    // خطوة جديدة
                    recipe.Directions.Add(new Direction
                    {
                        DirectionText = dir.DirectionText,
                        StepNumber = recipe.Directions.Count + 1, 
                        RecipeId = recipe.Id
                    });
                }
                else
                {
                    // خطوة موجودة
                    var existingDir = recipe.Directions.FirstOrDefault(d => d.Id == dir.Id);
                    if (existingDir != null)
                    {
                        existingDir.DirectionText = dir.DirectionText;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Recipe");
        }



        [HttpPost]
        public async Task<IActionResult> DeleteRecipes(int id)
        {
            var Recipes = await _context.Recipes.FindAsync(id);
            if (Recipes == null)
                return NotFound();

            _context.Recipes.Remove(Recipes);
            await _context.SaveChangesAsync();

            return RedirectToAction("Recipe");
        }

        public IActionResult AddRecipe()
        {
            return View();

        }
        [HttpPost]
        public IActionResult AddRecipe(Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _context.Recipes.Add(recipe);
                _context.SaveChanges();

                return RedirectToAction("Recipe");
            }

            return View(recipe);
        }
    }
}
