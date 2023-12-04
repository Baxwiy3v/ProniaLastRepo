namespace AB460Proniya.Models
{
    public class ProductSize
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public Size Size { get; set; }
        public Product Product { get; set; }
    }
}
