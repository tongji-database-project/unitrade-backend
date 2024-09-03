using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///关系集，用于记录买家发起的订单
    ///</summary>
    public partial class PLACES
    {
           public PLACES(){


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
           public string ORDER_ID {get;set;}

    }
}
