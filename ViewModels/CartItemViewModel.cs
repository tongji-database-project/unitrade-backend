namespace UniTrade.ViewModels
{
    public class CartItemViewModel
    {
        public string merchandise_id { get; set; }
        public string merchandise_name {  get; set; }
        public double merchandise_price { get; set; }
        public string picture {  get; set; }
        public int quanity { get; set; }
        public DateTime cart_time { get; set; }
        public bool selected { get; set; }
    }
}
