namespace UniTrade.ViewModels
{
    public class PublishProductViewModel
    {
        public string name { get; set; }
        public decimal price { get; set; }
        public long inventory { get; set; }
        public string type { get; set; }
        public string cover_image_url { get; set; }  // 封面图片的URL
        public List<string> product_image_urls { get; set; }  // 多个商品图片的URL
        public string product_details { get; set; }
    }
}
