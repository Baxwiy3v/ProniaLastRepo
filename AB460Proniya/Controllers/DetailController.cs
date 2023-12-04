using AB460Proniya.DAL;
using AB460Proniya.Models;
using AB460Proniya.ModelsVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB460Proniya.Controllers
{
    public class DetailController : Controller
    {
      
        private readonly AppDbContext _context;

        public DetailController(AppDbContext context)
        {
            _context = context;
        }

		public async Task<IActionResult> Detail(int id)
		{
            if (id <= 0) return BadRequest();

            Product product = await _context.Products

                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags)
                .ThenInclude(p => p.Tag)
                .Include(p => p.ProductSizes)
                .ThenInclude(p => p.Size)
                .Include(p => p.ProductColors)
                .ThenInclude(p => p.Color)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            List<Product> RelatedProducts = await _context.Products
                .Include(pi => pi.ProductImages
                .Where(pi => pi.IsPrimary != null))
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .ToListAsync();

            DetailVM vm = new DetailVM()
            {
                Product = product,

                RelatedProducts = RelatedProducts
            };
           



            return View(vm);

        }
    }
}
