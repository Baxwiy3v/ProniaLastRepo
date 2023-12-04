using AB460Proniya.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB460Proniya.ViewComponents
{
	public class FooterViewComponent : ViewComponent
	{
		private readonly AppDbContext _context;

		public FooterViewComponent(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			Dictionary<string, string> settings = await _context.Settings
				.ToDictionaryAsync(d => d.Key, d => d.Value);

			return View(settings);
		}
	}
}
