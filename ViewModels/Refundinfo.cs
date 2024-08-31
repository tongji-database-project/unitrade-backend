namespace UniTrade.ViewModels
{
    public class QueryRefundInfo
    {
        public string refund_id { get; set; }
        public string seller_id { get; set; }
        public string seller_name { get; set; }
        public string buyer_id { get; set; }
        public string buyer_name { get; set; }
        public string merchandise_name { get; set; }
        public string reason { get; set; }
        public System.DateTime time { get; set; }
    }

    public class AuditRefundInfo
    {
        public string refund_id { get; set; }
        public bool is_agreed { get; set; }
    }

}
