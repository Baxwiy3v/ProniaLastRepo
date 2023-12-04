using AB460Proniya.Areas.ViewModels;
using AB460Proniya.DAL;
using AB460Proniya.Models;
using AB460Proniya.Utilities.Extendions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB460Proniya.Areas.admin.Controllers
{
	[Area("admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages
                .Where(pi => pi.IsPrimary == true))
                .Include(pt => pt.ProductTags)
                .ThenInclude(pt => pt.Tag)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                return View(productVM);
            }
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("CategoryId", "There is no such category");
                return View(productVM);
            }

            foreach (int id in productVM.TagIds)
            {
                bool TagResult = await _context.Tags.AnyAsync(t => t.Id == id);
                if (!TagResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("TagIds", "There is no such tag");
                    return View(productVM);
                }
            }
            foreach (int id in productVM.ColorIds)
            {
                bool colorResult = await _context.Colors.AnyAsync(t => t.Id == id);
                if (!colorResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("ColorIds", "There is no such color");
                    return View(productVM);
                }
            }
            foreach (int id in productVM.SizeIds)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == id);
                if (!sizeResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("SizeIds", "There is no such size");
                    return View(productVM);
                }
            }

            if (!productVM.MainPhoto.ValidateType())
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("MainPhoto", "Wrong file type");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(600))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("MainPhoto", "Wrong file size");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.ValidateType())
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "Wrong file type");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.ValidateSize(600))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "Wrong file size");
                return View(productVM);
            }
            ProductImage image = new ProductImage
            {
                Alternative = productVM.Name,
                IsPrimary = true,
                Url = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
            };
            ProductImage hoverImage = new ProductImage
            {
                Alternative = productVM.Name,
                IsPrimary = false,
                Url = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
            };


            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                CategoryId = (int)productVM.CategoryId,
                Description = productVM.Description,
                ProductTags = new(),
                ProductColors = new(),
                ProductSizes = new(),
                ProductImages = new()
                {
                    image,hoverImage
                }
            };

            foreach (int id in productVM.TagIds)
            {
                var pTag = new ProductTag
                {
                    TagId = id,
                    Product = product
                };
                product.ProductTags.Add(pTag);
            }
            foreach (int id in productVM.ColorIds)
            {
                var pColor = new ProductColor
                {
                    ColorId = id,
                    Product = product
                };
                product.ProductColors.Add(pColor);
            }
            foreach (int id in productVM.SizeIds)
            {
                var pSize = new ProductSize
                {
                    SizeId = id,
                    Product = product
                };
                product.ProductSizes.Add(pSize);
            }

            TempData["Message"] = "";
            foreach (IFormFile photo in productVM.Photos)
            {
                if (!photo.ValidateType())
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
                    continue;
                }
                if (!photo.ValidateSize(600))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
                    continue;
                }

                product.ProductImages.Add(new ProductImage
                {
                    Alternative = product.Name,
                    IsPrimary = null,
                    Url = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
                });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(pt => pt.ProductTags)
                .Include(s => s.ProductSizes)
                .Include(p => p.ProductImages)
                .Include(c => c.ProductColors)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (product is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Price = product.Price,
                CategoryId = (int)product.CategoryId,
                TagIds = product.ProductTags.Select(t => t.TagId).ToList(),
                ProductImages = product.ProductImages,
                SizeIds = product.ProductSizes.Select(s => s.SizeId).ToList(),
                ColorIds = product.ProductColors.Select(c => c.ColorId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                Colors = await _context.Colors.ToListAsync()
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _context.Products
                .Include(p => p.ProductTags)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == id);

            productVM.ProductImages = existed.ProductImages;
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                return View(productVM);
            }

            if (existed is null) return NotFound();

            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);

            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bu adda category movcud deyil");
                return View(productVM);
            }

            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "Fayl novu uygun deyil");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(600))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "Fayl olcusu uygun deyil:600kB");
                    return View(productVM);
                }
            }

            if (productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.ValidateType("image/"))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("HoverPhoto", "Fayl novu uygun deyil");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.ValidateSize(600))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("HoverPhoto", "Fayl olcusu uygun deyil:600kB");
                    return View(productVM);
                }
            }

            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                _context.ProductImages.Remove(mainImage);

                existed.ProductImages.Add(new ProductImage
                {
                    Alternative = productVM.Name,
                    IsPrimary = true,
                    Url = fileName
                });
            }
            if (productVM.HoverPhoto is not null)
            {
                string fileName = await productVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage hoverImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                hoverImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                _context.ProductImages.Remove(hoverImage);

                existed.ProductImages.Add(new ProductImage
                {
                    Alternative = productVM.Name,
                    IsPrimary = false,
                    Url = fileName
                });
            }

            List<ProductImage> removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImage pImage in removeable)
            {
                pImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(pImage);
            }

            TempData["Message"] = "";
            if (productVM.Photos is not null)
            {
                foreach (IFormFile photo in productVM.Photos)
                {
                    if (!photo.ValidateType("image/"))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil</p>";
                        continue;
                    }

                    if (!photo.ValidateSize(500))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil</p>";
                        continue;
                    }
                    existed.ProductImages.Add(new ProductImage
                    {
                        Alternative = productVM.Name,
                        IsPrimary = null,
                        Url = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
                    });
                }
            }

            existed.ProductTags.RemoveAll(pt => !productVM.TagIds.Exists(tId => tId == pt.TagId));
            List<int> tagCreatable = productVM.TagIds.Where(tId => !existed.ProductTags.Exists(pt => pt.TagId == tId)).ToList();
            foreach (int tagId in tagCreatable)
            {
                bool tagResult = await _context.Tags.AnyAsync(t => t.Id == tagId);
                if (!tagResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("TagId", "Bu adda tag movcud deyil");
                    return View(productVM);
                }
                existed.ProductTags.Add(new ProductTag
                {
                    TagId = tagId
                });
            }

           

            existed.ProductColors.RemoveAll(pc => !productVM.ColorIds.Exists(cId => cId == pc.ColorId));

            List<int> colorCreatable = productVM.ColorIds.Where(cId => !existed.ProductColors.Exists(pc => pc.ColorId == cId)).ToList();

            foreach (int colorId in colorCreatable)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(c => c.Id == colorId);
                if (!sizeResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("ColorId", "Bu adda color movcud deyil");
                    return View();
                }
                existed.ProductColors.Add(new ProductColor
                {
                    ColorId = colorId
                });
            }

            

            existed.ProductSizes.RemoveAll(pt => !productVM.SizeIds.Exists(sId => sId == pt.SizeId));

            

            List<int> sizeCreatable = productVM.SizeIds.Where(sId => !existed.ProductSizes.Exists(ps => ps.SizeId == sId)).ToList();

            foreach (int sizeId in sizeCreatable)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(s => s.Id == sizeId);
                if (!sizeResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("SizeId", "Bu adda size movcud deyil");
                    return View();
                }
                existed.ProductSizes.Add(new ProductSize
                {
                    SizeId = sizeId
                });
            }


            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.SKU = productVM.SKU;
            existed.CategoryId = (int)productVM.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(c => c.Id == id);

            if (product is null) return NotFound();

            foreach (ProductImage image in product.ProductImages)
            {
                image.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            };

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            return View(product);
        }
    }
}
