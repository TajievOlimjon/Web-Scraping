namespace WebApi
{
    public class AttributeDetail
    {
        public int Id { get; set; }
        public ProductAttributeDetail ProductAttributeDetail { get; set; } = new();
        public int AttributeId { get; set; }
    }
    public class ProductAttributeDetail
    {
        public string? Name { get; set; } = null;
        public string? NameValue { get; set; } = null;
    }
}





