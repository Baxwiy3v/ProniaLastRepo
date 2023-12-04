using System.ComponentModel.DataAnnotations;

namespace AB460Proniya.ModelsVM
{
    public class RegisterVM 
    {
        
        [Required (ErrorMessage= "Ad tələb olunur")]
        [MinLength(3,ErrorMessage ="Minimum uzunluğu 3 olmalıdır")]
        [MaxLength(25,ErrorMessage ="Maksimum uzunluğu 25 olmalıdır")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad tələb olunur")]
        [MinLength(3,ErrorMessage = "Minimum uzunluğu 3 olmalıdır")]
        [MaxLength(25, ErrorMessage = "Maksimum uzunluğu 25 olmalıdır")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "İstifadəçi adı tələb olunur")]
        [MinLength(3, ErrorMessage = "Minimum uzunluğu 3 olmalıdır")]
        [MaxLength(25,ErrorMessage = "Maksimum uzunluğu 25 olmalıdır")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Email adı tələb olunur")]
        [MaxLength(255, ErrorMessage = "Maksimum uzunluğu 256 olmalıdır")]
        [DataType(DataType.EmailAddress)]
		[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "E-mail ünvanı səhvdir")]
		public string Email { get; set; }


        public string Gender { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare (nameof(Password))]
        public string ConfirmPassword { get; set; }





    }
}
