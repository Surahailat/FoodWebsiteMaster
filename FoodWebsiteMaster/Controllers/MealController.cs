using FoodWebsiteMaster.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsiteMaster.Controllers
{
    public class MealController : Controller
    {
        private readonly MyDbContext _context;

        public MealController(MyDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> mealPlan()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("signIn", "user");
            }

            var now = DateTime.Now;

            // تحقق إذا المستخدم لديه اشتراك ساري
            bool hasSubscription = await _context.UserSubscribes
                .AnyAsync(us => us.UserId == userId && us.StartDate <= now && us.EndDate >= now);

            if (!hasSubscription)
            {
                // المستخدم ليس لديه اشتراك ساري
                return RedirectToAction("SubscriptionAlert");
            }

            // المستخدم مشترك، أعرض له الوجبات
            var recipes = await _context.Recipes.ToListAsync();
            return View(recipes);
        }
        public IActionResult SubscriptionAlert()
        {
            return View(); // فيه السويت ألرت
        }

        public IActionResult RecipeDetails(int id)
        {
            var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id);
            var directions = _context.Directions
                .Where(d => d.RecipeId == id)
                .OrderBy(d => d.StepNumber)
                .ToList();

            var ingredients = _context.Ingredients
                .Where(i => i.RecipeId == id)
                .ToList();

            var viewModel = new RecipeDetailsViewModel
            {
                Recipe = recipe,
                Directions = directions,
                Ingredients = ingredients
            };

            return View(viewModel);
        }

    }
}
