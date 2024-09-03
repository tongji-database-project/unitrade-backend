namespace UniTrade.ViewModels
{
    public class QueryAppealInfo
    {
        public string appeal_id { get; set; }
        public string seller_id { get; set; }
        public string seller_name { get; set; }
        public string complainant_id { get; set; }
        public string complainant_name { get; set; }
        public short credibility { get; set; }
        public string reason { get; set; }
        public System.DateTime time { get; set; }
    }

    public class AuditAppealInfo
    {
        public string appeal_id { get; set; }
        public bool is_agreed { get; set; }
    }

}
