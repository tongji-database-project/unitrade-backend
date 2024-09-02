namespace UniTrade.ViewModels
{
    public class CartItemViewModel
    {
        public string UserId { get; set; }
        public string MerchandiseId { get; set; }
        public int Quantity { get; set; }
        public DateTime CartTime { get; set; }
        public bool Selected { get; set; }
    }
}
