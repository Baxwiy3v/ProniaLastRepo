using AB460Proniya.DAL;
using AB460Proniya.Models;
using AB460Proniya.ModelsVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AB460Proniya.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;
		private readonly UserManager<AppUser> _userManager;

		public HeaderViewComponent(AppDbContext context, IHttpContextAccessor accessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _accessor = accessor;
			_userManager = userManager;
		}

        public async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderVM vm = new();
            vm.Settings = await _context.Settings
                .ToDictionaryAsync(d => d.Key, d => d.Value);

            vm.Basket = await GetBasketItems();

            return View(vm);
        }

        public async Task<List<BasketItemVM>> GetBasketItems()
        {
			List<BasketItemVM> basketVM = new();

			if (_accessor.HttpContext.User.Identity.IsAuthenticated)
			{
				var user = await _userManager.Users
					.Include(u => u.BasketItems)
					.ThenInclude(bi => bi.Product)
					.ThenInclude(p => p.ProductImages
					.Where(pi => pi.IsPrimary == true))
					.FirstOrDefaultAsync(u => u.Id == _accessor.HttpContext.User
					.FindFirstValue(ClaimTypes.NameIdentifier));

				foreach (BasketItem item in user.BasketItems)
				{
					basketVM.Add(new BasketItemVM()
					{
						Name = item.Product.Name,
						Price = item.Product.Price,
						Count = item.Count,
						SubTotal = item.Count * item.Product.Price,
						Image = item.Product.ProductImages.FirstOrDefault().Url
					});
				}
			}
			else

			{

				if (_accessor.HttpContext.Request.Cookies["Basket"] is not null)
				{
					List<BasketCookieItemVM> basket = JsonConvert
						.DeserializeObject<List<BasketCookieItemVM>>(_accessor.HttpContext.Request.Cookies["Basket"]);

					foreach (BasketCookieItemVM basketCookie in basket)
					{
						var product = await _context.Products
							.Include(p => p.ProductImages
							.Where(pi => pi.IsPrimary == true))
							.FirstOrDefaultAsync(p => p.Id == basketCookie.Id);

						if (product is not null)
						{
							BasketItemVM basketItem = new()
							{
								Id = product.Id,

								Name = product.Name,

								Image = product.ProductImages.FirstOrDefault().Url,

								Price = product.Price,

								Count = basketCookie.Count,

								SubTotal = product.Price * basketCookie.Count,
							};
							basketVM.Add(basketItem);

						}
					}
				}
			}

			return basketVM;
		}
    }
}
