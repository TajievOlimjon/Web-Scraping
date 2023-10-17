namespace WebApi
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}



