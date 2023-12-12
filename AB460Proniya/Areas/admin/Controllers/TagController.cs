﻿using AB460Proniya.Areas.admin.ViewModels;
using AB460Proniya.Areas.ViewModels;
using AB460Proniya.DAL;
using AB460Proniya.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB460Proniya.Areas.ProniaAdmin.Controllers
{
    [Area("admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Tags.CountAsync();

            var Tags = await _context.Tags.Skip(page * 2).Take(2)

                .Include(t => t.ProductTags).ToListAsync();

            PaginationVM<Tag> pagination = new PaginationVM<Tag>()
            {
                CurrentPage = page,

                TotalPage = Math.Ceiling(count / 2),

                Items = Tags
            };
            return View(pagination);
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid) return View();
            
            bool outcome = await _context.Tags
                .AnyAsync(c => c.Name
                .ToLower().Trim() == tagVM.Name
                .ToLower().Trim());

            if (outcome)
            {
                ModelState.AddModelError("Name", "his tag already exists");

                return View();
            }
            Tag tag = new Tag
            {
                Name = tagVM.Name,
            };

            await _context.Tags.AddAsync(tag);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Tag tag = await _context.Tags
                .FirstOrDefaultAsync(c => c.Id == id);

            if (tag is null) return NotFound();

            var vm = new UpdateTagVM
            {
                Name = tag.Name,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
        {
            if (!ModelState.IsValid) return View();

            Tag existed = await _context.Tags
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool outcome = _context.Tags.Any(c => c.Name == tagVM.Name && c.Id != id);
            if (outcome)
            {
                ModelState.AddModelError("Name", " artıq belə bir etiket var");

                return View();
            }

            existed.Name = tagVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id,bool confirim)
        {
            if (id <= 0) return BadRequest();

            var existed = await _context.Tags
                .FirstOrDefaultAsync(c => c.Id == id);


            if (existed is null) return NotFound();

            if (confirim)
            {
                _context.Tags.Remove(existed);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            else
            {
                return View(existed);
            }
            
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var tag = await _context.Tags

                .Include(c => c.ProductTags).
                ThenInclude(p => p.Product).
                ThenInclude(p => p.ProductImages).
                FirstOrDefaultAsync(s => s.Id == id);

            if (tag == null) return NotFound();

            return View(tag);
        }
    }
}
