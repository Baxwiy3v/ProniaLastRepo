using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System;
using AB460Proniya.DAL;
using AB460Proniya.Models;
using AB460Proniya.ModelsVM;
using Microsoft.EntityFrameworkCore;

namespace AB460Proniya.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Slide> Slides = await _context.Slides
                .OrderBy(s => s.Order)
                .Take(3)
                .ToListAsync();


			List<Product> Products = await _context.Products
                .Include(p => p.ProductImages)
                .ToListAsync();


			HomeViewModel vm = new()
            {
                Slides = Slides,
                Products = Products
            };
            return View(vm);

        }

        public IActionResult About()
        {
            return View();
        }
    }
}