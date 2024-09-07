namespace UniTrade.ViewModels
{
    public class RefundRequestViewModel
    {
        public string order_id { get; set; }
        public string refund_reason { get; set; }
        public string refund_feedback { get; set; }
        public string refund_state { get; set; }
    }
}
