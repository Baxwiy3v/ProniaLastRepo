


using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AB460Proniya.Areas.ViewModels
{

	public class UpdateColorVM
	{
		[Required(ErrorMessage = "Ad tələb olunur")]
		[MaxLength(25, ErrorMessage = "Adın maksimum uzunluğu 25-dir5")]
		public string Name { get; set; }
	}
}