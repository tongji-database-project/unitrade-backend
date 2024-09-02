using System.Collections.Generic;

namespace UniTrade.ViewModels
{
    public class OrderSummaryViewModel
    {
        public string UserName { get; set; } // 用户名
        public string Phone { get; set; } // 联系电话
        public string Address { get; set; } // 收货地址
        public List<MerchandiseViewModel> CartItems { get; set; } // 购物车选中商品列表
        public decimal TotalPrice { get; set; } // 商品总价
        public decimal ShippingFee { get; set; } // 运费
        public decimal GrandTotal { get; set; } // 总金额（商品总价 + 运费）
    }

    public class MerchandiseViewModel
    {
        public string MerchandiseId { get; set; } // 商品ID
        public string Name { get; set; } // 商品名称
        public long Quantity { get; set; } // 商品数量
        public decimal Price { get; set; } // 商品单价
        public decimal Subtotal { get; set; } // 小计 (Price * Quantity)
    }
}