using System.Collections.Generic;

namespace UniTrade.ViewModels
{
    public class OrderSummaryViewModel
    {
        public string user_name { get; set; } // 用户名
        public string phone { get; set; } // 联系电话
        public string address { get; set; } // 收货地址
        public List<CartItemViewModel> cart_items { get; set; } // 购物车选中商品列表
        public decimal total_price { get; set; } // 商品总价
        public decimal shipping_fee { get; set; } // 运费
        public decimal grand_total { get; set; } // 总金额（商品总价 + 运费）
    }
}