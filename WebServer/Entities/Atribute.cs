namespace WebApi
{
    public class Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<AttributeDetail>? AttributeDetails { get; set; }
    }
}






