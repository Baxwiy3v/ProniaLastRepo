using AB460Proniya.Models;

namespace AB460Proniya.ModelsVM
{
    public class OrderVM
    {
        public string Address { get; set; }
        public List<BasketItem>? BasketItems { get; set; }
    }
}
