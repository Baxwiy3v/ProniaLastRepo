using AB460Proniya.DAL;
using AB460Proniya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB460Proniya.ViewComponents
{
	public class ProductViewComponent : ViewComponent
	{
		private readonly AppDbContext _context;

		public ProductViewComponent(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync(int key = 1)
		{
			List<Product> products;
			switch (key)
			{
				case 1:
					products = await

				 _context.Products
				 
				.OrderBy(p => p.Name)
				.Take(10)
				.Include(p => p.ProductImages
				.Where(pi => pi.IsPrimary != null))
				.ToListAsync();
					break;

				case 2:

					products = await

				 _context.Products
				 .OrderByDescending(p => p.Price)
				.Take(10)
				.Include(p => p.ProductImages
				.Where(pi => pi.IsPrimary != null))
				.ToListAsync();
					break;

				case 3:
					products = await

				_context.Products
			   .OrderByDescending(p => p.Id)
			   .Take(10)
			   .Include(p => p.ProductImages
			   .Where(pi => pi.IsPrimary != null))
			   .ToListAsync();
					break;

				default:

					products = await

				_context.Products

			   .Take(10)
			   .Include(p => p.ProductImages
			   .Where(pi => pi.IsPrimary != null))
			   .ToListAsync();

					break;
			}
			return View(products);
		}
	}
}
