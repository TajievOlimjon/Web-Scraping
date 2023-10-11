namespace WebApi
{
    public class Attribute
    {
        public int Id { get; set; }
        public string? AttributeName { get; set; }
        public int ProductAboutId { get; set; }
        public List<AttributeDetail>? AttributeDetails { get; set; } = null;
    }
}





