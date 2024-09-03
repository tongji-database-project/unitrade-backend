using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///订单表，用于存放订单的详情信息，订单中如有多个商品，则分开存为不同项
    ///</summary>
    public partial class ORDERS
    {
           public ORDERS(){


           }
           /// <summary>
           /// Desc:订单ID可能不唯一，故主码还需要加上商品ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ORDER_ID {get;set;}

           /// <summary>
           /// Desc:使用三个字符描述订单状态
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string STATE {get;set;}

           /// <summary>
           /// Desc:订单包含的其中一个商品
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string MERCHANDISE_ID {get;set;}

           /// <summary>
           /// Desc:该订单中对应商品的数量
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long ORDER_QUANITY {get;set;}

           /// <summary>
           /// Desc:下单时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime ORDER_TIME {get;set;}

           /// <summary>
           /// Desc:到货时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? RECEIVING_TIME {get;set;}

    }
}
