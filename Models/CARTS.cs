using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///关系集，购物车表，存放用户购物车中的商品信息
    ///</summary>
    public partial class CARTS
    {
           public CARTS(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string CUSTOMER_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string MERCHANDISE_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long QUANITY {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime CART_TIME {get;set;}

           /// <summary>
           /// Desc:用于标记是否被选中处于预支付状态
           /// Default:
           /// Nullable:False
           /// </summary>           
           public bool SELECTED {get;set;}

    }
}
