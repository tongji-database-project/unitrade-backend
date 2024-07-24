using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///关系集，用于存放卖家成交订单的情况
    ///</summary>
    public partial class HOLDS
    {
           public HOLDS(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SELLER_ID {get;set;}

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
           public string ORDER_ID {get;set;}

    }
}
