namespace WebApi
{
    public class Attribute
    {
        public int Id { get; set; }
        public string AttributeName { get; set; } = null!;
        public int ProductId { get; set; }
        public List<AttributeDetail> AttributeDetails { get; set; } = new();
    }
}





