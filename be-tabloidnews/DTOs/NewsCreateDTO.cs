namespace be_tabloidnews.DTOs
{
    public class NewsCreateDTO
    {
        public string title { get; set; }
        public string topic { get; set; }
        public string content { get; set; }
        public string author { get; set; }
        public string[] tags { get; set; }
        public string bannerImg { get; set; }
        public string description { get; set; }
    }
}
