using System.ComponentModel.DataAnnotations;

namespace AB460Proniya.ModelsVM
{
    public class LoginVM
    {
        [Required(ErrorMessage= "User ve ye Email daxil edilmelidir")]

        public string UserOrEmail { get; set; }
        [Required(ErrorMessage = "Password daxil edilmelidir")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsRemember { get; set; }
    }
}
