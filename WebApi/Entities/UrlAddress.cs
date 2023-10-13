namespace WebApi
{
    public class UrlAddress
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public bool VisitedAddress { get; set; } = false;
    }
}

