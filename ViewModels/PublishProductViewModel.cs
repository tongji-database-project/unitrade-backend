namespace UniTrade.ViewModels
{
    public class PublishProductViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public long Inventory { get; set; }
        public string Type { get; set; }
        public IFormFile Picture { get; set; }
    }
}