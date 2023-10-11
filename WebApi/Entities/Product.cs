namespace WebApi
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public int CategoryId { get; set; }
        public List<ProductAbout>? ProductAbouts { get; set; } = null;
    }
}




