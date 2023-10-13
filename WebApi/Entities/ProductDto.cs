namespace WebApi
{
    public class ProductDto
    {
        public string? Url { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public string? Price { get; set; }
    }
    public class GetAllCategoriesDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public string? FileName { get; set; } = null;
        public List<GetAllProductsDto>? Products { get; set; } = new();
    }
    public class GetAllProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? FileName { get; set; } = null;
        public int CategoryId { get; set; }
        public List<GetAllAttributesDto> AllAttributes { get; set; } = new();
    }
    public class GetAllAttributesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<GetAllAttributeDetailsDto> Details { get; set; } = new();
    }
    public class GetAllAttributeDetailsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public string? NameValue { get; set; } = null;
    }
}






