using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi
{
    public class AttributeDetail
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public int AttributeId { get; set; }
        public virtual Attribute Attribute { get; set; } = null!;
        public virtual ICollection<ProductAttributeDetail>? ProductAttributeDetails { get; set; }
    }
}








