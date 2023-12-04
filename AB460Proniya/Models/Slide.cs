using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AB460Proniya.Models
{
    public class Slide
    {
        public int Id { get; set; }
		[Required(ErrorMessage = "Başlıq tələb olunur")]
		[MaxLength(25, ErrorMessage = "Maksimum uzunluq 25-dir")]
		public string Title { get; set; }
		[Required(ErrorMessage = "Başlıq tələb olunur")]

		[MaxLength(60, ErrorMessage = "Maksimum uzunluq 60-di")]
		public string Subtitle { get; set; }

		[Required(ErrorMessage = "Başlıq tələb olunur")]

		[MaxLength(80, ErrorMessage = "Maksimum uzunluq 80-di")]
		public string Description { get; set; }

        public int Order { get; set; }
        public string? ImageUrl { get; set; }
		[NotMapped]
		public IFormFile? Photo { get; set; }
	}
}
