using System.ComponentModel.DataAnnotations;

namespace WebApi
{
    public class AttributeDetail
    {
        public int Id { get; set; }
        [DataType("jsonb")]
        public ProductAttributeDetailName ProductAttributeDetailName { get; set; } = new();
        public int AttributeId { get; set; }
    }
    public class ProductAttributeDetailName
    {
        public string? Name { get; set; } = null;
        public string? NameValue { get; set; } = null;
    }
}






