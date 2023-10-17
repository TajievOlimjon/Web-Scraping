namespace WebApi
{
    public class ProductAttributeDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int AttributeDetailId { get; set; }
        public virtual AttributeDetail AttributeDetail { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}







