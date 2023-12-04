using AB460Proniya.DAL;
using AB460Proniya.Models;
using AB460Proniya.ModelsVM;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AB460Proniya.Services
{
	public class LayoutService
	{
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;

        public LayoutService(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            var settings = await _context.Settings
                .ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }

        public async Task<List<BasketItemVM>> GetBasketItemsAsyns()
        {
            List<BasketItemVM> basketVM = new List<BasketItemVM>();
            if (_http.HttpContext.Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> basket = JsonConvert
                    .DeserializeObject<List<BasketCookieItemVM>>(_http.HttpContext.Request.Cookies["Basket"]);

                foreach (BasketCookieItemVM basketCookieItem in basket)
                {
                    Product product = await _context.Products
                        .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                        .FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);

                    if (product is not null)
                    {
                        BasketItemVM basketItemVM = new BasketItemVM
                        {
                            Id = product.Id,

                            Name = product.Name,

                            Image = product.ProductImages
                            .FirstOrDefault().Url,

                            Price = product.Price,

                            Count = basketCookieItem.Count,
                            SubTotal = product.Price * basketCookieItem.Count,
                        };

                        basketVM.Add(basketItemVM);

                    }
                }
            }
            return basketVM;
        }
    }
}
