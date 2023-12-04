


using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace AB460Proniya.Areas.ViewModels
{
	public class UpdateTagVM
	{
		[Required(ErrorMessage = "Ad tələb olunurd")]
		[MaxLength(25, ErrorMessage = "Adın maksimum uzunluğu 25-dir")]
		public string Name { get; set; }
	}
}




