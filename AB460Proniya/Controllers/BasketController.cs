using AB460Proniya.DAL;
using AB460Proniya.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AB460Proniya.ModelsVM;
using Microsoft.EntityFrameworkCore;

namespace AB460Proniya.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM = new List<BasketItemVM>();

            if (Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> basket = JsonConvert
                    .DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                foreach (BasketCookieItemVM basketCookieItem in basket)
                {
                    var product = await _context.Products
                        .Include(p => p.ProductImages
                        .Where(pi => pi.IsPrimary == true))
                        .FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);

                    if (product is not null)
                    {
                        BasketItemVM basketItemVM = new BasketItemVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Image = product.ProductImages.FirstOrDefault().Url,
                            Price = product.Price,
                            Count = basketCookieItem.Count,
                            SubTotal = product.Price * basketCookieItem.Count,
                        };

                        basketVM.Add(basketItemVM);

                    }
                }
            }
            return View(basketVM);
        }

        public async Task<IActionResult> AddBasket(int id, string plus)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            List<BasketCookieItemVM> basket;

            if (Request.Cookies["Basket"] is not null)
            {
                basket = JsonConvert
                    .DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                BasketCookieItemVM itemVM = basket
                    .FirstOrDefault(b => b.Id == id);

                if (itemVM is null)
                {
                    BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };

                    basket.Add(basketCookieItemVM);
                }
                else
                {
                    itemVM.Count++;
                }
            }
            else
            {
                basket = new List<BasketCookieItemVM>();

                BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                {
                    Id = id,
                    Count = 1
                };

                basket.Add(basketCookieItemVM);
            }

            string json = JsonConvert.SerializeObject(basket);

            Response.Cookies.Append("Basket", json);

            return RedirectToAction(nameof(Index), "plus");
        }
        public async Task<IActionResult> MinusBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            List<BasketCookieItemVM> basket;
            if (Request.Cookies["Basket"] is not null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                BasketCookieItemVM item = basket.FirstOrDefault(b => b.Id == id);
                if (item is not null)
                {
                    item.Count--;
                    if (item.Count == 0)
                    {
                        basket.Remove(item);
                    }
                    string json = JsonConvert.SerializeObject(basket);
                    Response.Cookies.Append("Basket", json);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RemoveBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            List<BasketCookieItemVM> basket;
            if (Request.Cookies["Basket"] is not null)
            {
                basket = JsonConvert
                    .DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                var outcome = basket
                    .FirstOrDefault(b => b.Id == id);

                if (outcome is not null)
                {
                    basket.Remove(outcome);

                    string json = JsonConvert.SerializeObject(basket);

                    Response.Cookies.Append("Basket", json);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
