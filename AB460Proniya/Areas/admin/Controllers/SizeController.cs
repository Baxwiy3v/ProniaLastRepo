using AB460Proniya.Areas.ViewModels;
using AB460Proniya.DAL;
using AB460Proniya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Size = AB460Proniya.Models.Size;

namespace AB460Proniya.Areas.admin.Controllers
{
    [Area("admin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var size = await _context.Sizes
                .Include(s => s.ProductSizes)
                .ToListAsync();

            return View(size);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if (!ModelState.IsValid) return View();
            
            bool outcome =await _context.Sizes
                .AnyAsync(c => c.Name
                .ToLower().Trim() == sizeVM.Name
                .ToLower().Trim());

            if (outcome)
            {
                ModelState.AddModelError("Name", "Bu ölçü artıq mövcuddur");
                return View();
            }
            Size size = new Size
            {
                Name = sizeVM.Name,
            };
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Size size = await _context.Sizes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (size is null) return NotFound();

            UpdateSizeVM vm = new UpdateSizeVM
            {
                Name = size.Name
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSizeVM sizeVM)
        {
            if (!ModelState.IsValid) return View();

            Size existed = await _context.Sizes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool outcome = await _context.Sizes
                .AnyAsync(c => c.Name == sizeVM.Name && c.Id != id);

            if (outcome)
            {
                ModelState.AddModelError("Name", "Artıq belə bir ölçü var");
                return View();
            }

            existed.Name = sizeVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var existed = await _context.Sizes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _context.Sizes.Remove(existed);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var size = await _context.Sizes

                .Include(c => c.ProductSizes)
                .ThenInclude(pc => pc.Product)
                .ThenInclude(p => p.ProductImages).
                FirstOrDefaultAsync(s => s.Id == id);

            if (size == null) return NotFound();

            return View(size);
        }
    }
}
