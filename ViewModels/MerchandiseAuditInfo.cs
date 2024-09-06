namespace UniTrade.ViewModels
{
    public class MerchandiseAuditInfo
    {
        string merchandise_id { get; set; }
        string merchandise_name { get; set; }
        string merchandise_type { get; set; }
        decimal merchandise_price { get; set; }
        string seller_name { get; set; }
        short reputation { get; set; }
        int commit_sum { get; set; }
        float average_point { get; set; }
    }

    public class MerchandiseCommit
    {
        string merchandise_id { get; set; }
        int point { get; set; }
    }

    public class MerchandiseCommitStatistics
    {
        string merchandise_id { get; set; }
        int sum { get; set; }
        float average { get; set; }
    }
}
