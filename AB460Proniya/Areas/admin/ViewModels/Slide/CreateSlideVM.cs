



using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AB460Proniya.Areas.ViewModels
{

	public class CreateSlideVm
	{
		[Required(ErrorMessage = "Ad tələb olunur")]
		[MaxLength(25, ErrorMessage = "Adın maksimum uzunluğu 25-dir")]
		public string Title { get; set; }
		public string Description { get; set; }
        public string Subtitle { get; set; }
        public int Order { get; set; }
		public IFormFile? Photo { get; set; }
	}

}
	