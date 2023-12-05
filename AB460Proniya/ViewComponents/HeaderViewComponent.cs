using AB460Proniya.DAL;
using AB460Proniya.Models;
using AB460Proniya.ModelsVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AB460Proniya.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderVM vm = new();
            vm.Settings = await _context.Settings.ToDictionaryAsync(d => d.Key, d => d.Value);
            vm.Basket = await GetBasketItems();
            return View(vm);
        }

        public async Task<List<BasketItemVM>> GetBasketItems()
        {
            List<BasketItemVM> basketVM = new();
            if (_accessor.HttpContext.Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(_accessor.HttpContext.Request.Cookies["Basket"]);

                foreach (BasketCookieItemVM basketCookieItem in basket)
                {
                    Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);

                    if (product is not null)
                    {
                        BasketItemVM basketItemVM = new()
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
            return basketVM;
        }
    }
}
