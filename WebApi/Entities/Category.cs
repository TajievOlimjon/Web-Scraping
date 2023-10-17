namespace WebApi
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public string? Info { get; set; }
        public int? ParentId { get; set; }
        public virtual Category? Parent { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = null!;
    }
}




