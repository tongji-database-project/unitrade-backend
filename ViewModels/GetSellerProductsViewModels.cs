namespace UniTrade.ViewModels
{
    public class GetSellerProductsViewModels
    {
        public GetSellerProductsViewModels(string id, string name, decimal price, long inventory, string type, string cover_image_url, string product_details,long sales)
        {
            this.id = id;
            this.name = name;
            this.price = price;
            this.inventory = inventory;
            this.type = type;
            this.cover_image_url = cover_image_url;
            this.product_details = product_details;
            this.sales=sales;
        }

        public string id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public long inventory { get; set; }
        public string type { get; set; }
        public string cover_image_url { get; set; }  // 封面图片的URL
        public string product_details { get; set; }
        public long sales {  get; set; }
    }
}