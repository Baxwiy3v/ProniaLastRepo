using AB460Proniya.Areas.ViewModels;
using AB460Proniya.DAL;
using AB460Proniya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Color = AB460Proniya.Models.Color;

namespace AB460Proniya.Areas.admin.Controllers
{
    [Area("admin")]

    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var colors = await _context.Colors
                .Include(c => c.ProductColors)
                .ToListAsync();

            return View(colors);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid) return View();
            
            bool outcome = await _context.Colors
                .AnyAsync(c => c.Name
                .ToLower().Trim() == colorVM.Name
                .ToLower().Trim());

            if (outcome)
            {
                ModelState.AddModelError("Name", "Bu rəng artıq mövcuddur");
                return View();
            }

            Color color = new Color
            {
                Name = colorVM.Name
            };

            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Color color = await _context.Colors
                .FirstOrDefaultAsync(c => c.Id == id);

            if (color is null) return NotFound();

            UpdateColorVM vm = new UpdateColorVM
            {
                Name = color.Name
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {
            if (!ModelState.IsValid) return View();

            Color existed = await _context.Colors
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool outcome = await _context.Colors
                .AnyAsync(c => c.Name == colorVM.Name && c.Id != id);

            if (outcome)
            {
                ModelState.AddModelError("Name", "Artıq belə bir rəng var");
                return View();
            }

            existed.Name = colorVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var existed = await _context.Colors
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _context.Colors.Remove(existed);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var color = await _context.Colors
                .Include(c => c.ProductColors)
                .ThenInclude(pc => pc.Product)
                .ThenInclude(p => p.ProductImages).
                FirstOrDefaultAsync(s => s.Id == id);

            if (color == null) return NotFound();

            return View(color);
        }
    }

}
