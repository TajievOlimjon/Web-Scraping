namespace WebApi.Entities
{
    public class PageUrlAddress
    {
        public int Id { get; set; }
        public string PageUrl { get; set; } = null!;
        public bool VisitedAddress { get; set; } = false;
    }
}
