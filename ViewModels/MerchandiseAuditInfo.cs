namespace UniTrade.ViewModels
{
    public class MerchandiseAuditInfo
    {
        public string merchandise_id { get; set; }
        public string merchandise_name { get; set; }
        public string merchandise_type { get; set; }
        public decimal merchandise_price { get; set; }
        public string seller_name { get; set; }
        public short reputation { get; set; }
        public int commit_sum { get; set; }
        public double average_point { get; set; }
    }

    public class MerchandiseCommit
    {
        public string merchandise_id { get; set; }
        public int point { get; set; }
    }

    public class MerchandiseCommitStatistics
    {
        public string merchandise_id { get; set; }
        public int sum { get; set; }
        public double average { get; set; }
    }

    public class PullMerchandise
    {
        public string merchandise_id { get; set; }

    }

}
