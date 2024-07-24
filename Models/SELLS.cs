using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///关系集，用于记录卖家售卖的商品
    ///</summary>
    public partial class SELLS
    {
           public SELLS(){


           }
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
           public string SELLER_ID {get;set;}

    }
}
