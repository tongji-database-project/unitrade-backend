namespace UniTrade.ViewModels
{
    public class CommentInfo
    {

        public string content { get; set; }

        public DateTime time { get; set; }

        public IEnumerable<string> pictures { get; set; }

        public string user_avatar { get; set; }

        public string user_name { get; set; }
    }
}