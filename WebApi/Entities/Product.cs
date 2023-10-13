namespace WebApi
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? FileName { get; set; } = null;
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductAttributeDetail>? ProductAttributeDetails { get; set; }
    }
}








