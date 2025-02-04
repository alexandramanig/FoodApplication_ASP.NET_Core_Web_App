using Microsoft.AspNetCore.Mvc;
using FoodApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FoodApplication.ContectDBConfig;

namespace FoodApplication.Controllers
{
    public class RecipeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FoodApplicationDBContext _context;

        public RecipeController(UserManager<ApplicationUser> userManager, FoodApplicationDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetRecipeCard([FromBody] List<Recipe> recipes)
        {
            if (recipes == null || !recipes.Any())
            {
                return BadRequest(new { Error = "No recipes provided" });
            }
            return PartialView("_RecipeCard", recipes);
        }

        public IActionResult Search([FromQuery] string recipe)
        {
            if (string.IsNullOrWhiteSpace(recipe))
            {
                return RedirectToAction("Index");
            }

            ViewBag.Recipe = recipe;
            return View();
        }

        public IActionResult Order([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }

            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ShowOrder(OrderRecipeDetails orderRecipeDetails)
        {
            Random random = new Random();
            ViewBag.Price = Math.Round(random.Next(150, 500) / 5.0) * 5;

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }

            ViewBag.UserId = user.Id;
            ViewBag.Address = user.Address;
            return PartialView("_ShowOrder", orderRecipeDetails);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SubmitOrder([FromForm] Order order)
        {
            if (order == null)
            {
                return BadRequest(new { Error = "Invalid order data" });
            }

            order.OrderDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Recipe");
                }
                catch (Exception ex)
                {
                    // Log the exception
                    return StatusCode(500, new { Error = "Failed to save order", Details = ex.Message });
                }
            }

            return RedirectToAction("Order", "Recipe", new { id = order.Id.ToString() });
        }
    }
}
