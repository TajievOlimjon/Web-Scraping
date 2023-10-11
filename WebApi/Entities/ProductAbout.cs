namespace WebApi
{
    public class ProductAbout
    {
        public int Id { get; set; }
        public string? ProductAboutName { get; set; } = null;
        public int ProductId { get; set; }
        public List<Attribute>? Attributes { get; set; } = null;
    }
}




