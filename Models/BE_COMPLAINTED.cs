using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///用于存放被投诉商家与投诉申请的关系
    ///</summary>
    public partial class BE_COMPLAINTED
    {
           public BE_COMPLAINTED(){


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
           public string COMPLAINT_ID {get;set;}

    }
}
