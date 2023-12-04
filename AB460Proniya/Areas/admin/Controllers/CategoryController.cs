using AB460Proniya.Areas.ViewModels;
using AB460Proniya.DAL;
using AB460Proniya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB460Proniya.Areas.ProniaAdmin.Controllers
{
	[Area("admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> Categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return View(Categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return View();
           
            bool outcome = await _context.Categories
                .AnyAsync(c => c.Name
                .ToLower()
                .Trim() == categoryVM.Name.ToLower().Trim());

            if (outcome)
            {
                ModelState.AddModelError("Name", "Bu kateqoriya artıq mövcuddur");
                return View();
            }

            Category category = new Category
            {
                Name = categoryVM.Name,
            };

            await _context.Categories.AddAsync(category);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Category category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();

            UpdateCategoryVM vm = new UpdateCategoryVM
            {
                Name = category.Name,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryvm)
        {
            if (!ModelState.IsValid) return View();

            Category existed = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool outcome = await _context.Categories
                .AnyAsync(c => c.Name == categoryvm.Name && c.Id != id);

            if (outcome)
            {
                ModelState.AddModelError("Name", "Artıq belə bir kateqoriya var");
                return View();
            }

            existed.Name = categoryvm.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var existed = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();


            _context.Categories.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var category = await _context.Categories
                .Include(c => c.Products)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }
    }
}
