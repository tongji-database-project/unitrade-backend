using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///
    ///</summary>
    public partial class COMMUNICATION
    {
           public COMMUNICATION(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SELLER_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CUSTOMER_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime COMMUNICATION_TIME {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string COMMUNICATION_CONTENT {get;set;}

           /// <summary>
           /// Desc:通过一个字符标识该信息谁发出的，s表示卖家，c表示买家
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SENDER {get;set;}

    }
}
