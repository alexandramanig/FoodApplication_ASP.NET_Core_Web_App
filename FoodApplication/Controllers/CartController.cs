using Microsoft.AspNetCore.Mvc;
using FoodApplication.Models;
using FoodApplication.Repository;
using Microsoft.AspNetCore.Authorization;
using FoodApplication.ContectDBConfig;

namespace FoodApplication.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IData data;
        private readonly FoodApplicationDBContext context;

        public CartController(IData data, FoodApplicationDBContext context)
        {
            this.data = data;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveCart(Cart cart)
        {
            Console.WriteLine("Received Cart Data:");
            Console.WriteLine($"RecipeId: {cart.RecipeId}");
            Console.WriteLine($"Title: {cart.Title}");
            Console.WriteLine($"Image_url: {cart.Image_url}");
            Console.WriteLine($"Publisher: {cart.Publisher}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model State Errors:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Error: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                return BadRequest(new { Error = "Invalid cart data", ModelState });
            }

            var user = await data.GetUser(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }

            cart.UserId = user.Id;
            await context.Carts.AddAsync(cart);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Cart saved successfully" });
        }



        [HttpGet]
        public async Task<IActionResult> GetAddedCarts()
        {
            var user = await data.GetUser(HttpContext.User);
            var carts = context.Carts.Where(c=>c.UserId==user.Id).Select(c=>c.RecipeId).ToList();
            return Ok(carts);
        }
        [HttpPost]
        public IActionResult RemoveCartFromList(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var cart = context.Carts.Where(c => c.RecipeId == Id).FirstOrDefault();
                if (cart != null)
                {
                    context.Carts.Remove(cart);
                    context.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetCartList()
        {
            var user = await data.GetUser(HttpContext.User);
            var cartList = context.Carts.Where(c => c.UserId ==user.Id).Take(3).ToList();
            return PartialView("_CartList",cartList);
        }
    }
}
