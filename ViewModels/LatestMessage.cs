namespace UniTrade.ViewModel
{
    public class LatestMessageViewModel
    {
        public LatestMessageViewModel(string username, string content)
        {
            this.username = username;
            this.content = content;
        }

        public string username { get; set; }
        public string content { get; set; }
    }
}