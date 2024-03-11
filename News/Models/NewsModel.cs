namespace News.Models
{
    public class NewsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime Date { get; set; }
        public string ShortText { get; set; }
        public string FullText { get; set; }
    }

}
