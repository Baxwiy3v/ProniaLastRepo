using System.ComponentModel.DataAnnotations;

namespace AB460Proniya.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad tələb olunur")]

        [MaxLength(25, ErrorMessage = "Adın maksimum uzunluğu 25-dir")]

        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
