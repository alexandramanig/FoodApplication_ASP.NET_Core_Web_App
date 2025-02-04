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
            try
            {
                var user = await data.GetUser(HttpContext.User);
                if (user == null)
                {
                    return Unauthorized(); // Dacă utilizatorul nu este autentificat
                }

                cart.UserId = user.Id;

                if (ModelState.IsValid)
                {
                    await context.Carts.AddAsync(cart); // Asigură-te că utilizezi operațiunea asincronă
                    await context.SaveChangesAsync();   // Salvează modificările asincron
                    return Ok(new { Message = "Cart saved successfully" }); // Răspuns detaliat
                }

                return BadRequest(new { Error = "Invalid cart data", ModelState = ModelState });
            }
            catch (Exception ex)
            {
                // Loghează eroarea (poți folosi un serviciu de logare, dacă există)
                return StatusCode(500, new { Error = "An error occurred while saving the cart", Details = ex.Message });
            }
        }
    }
}
