namespace UniTrade.ViewModels
{
    public class OrderListViewModel
    {
        public string order_id {  get; set; }

        public string state { get; set; }

        public string merchandise { get; set; }

        public long order_quanity { get; set; }

        public DateTime order_time { get; set; }

        public DateTime receiving_time { get; set; }


    }
}
