namespace WebApi
{
    public class AdditionalProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public int ProductId { get; set; }
    }
}

