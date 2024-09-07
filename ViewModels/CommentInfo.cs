namespace UniTrade.ViewModels
{
    public class CommentInfo
    {

        public string content { get; set; }

        public DateTime time { get; set; }

        public IEnumerable<string> pictures { get; set; }

        public string user_avatar { get; set; }

        public string user_name { get; set; }

        public short quality { get; set; }

        public short attitude { get; set; }
      
        public short price { get; set; }
         
        public short logistic_speed { get; set; }
     
        public short conformity { get; set; }
    }
}