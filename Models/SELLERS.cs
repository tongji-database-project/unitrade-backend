using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///
    ///</summary>
    public partial class SELLERS
    {
           public SELLERS(){


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
           public string SELLER_NAME {get;set;}

           /// <summary>
           /// Desc:使用SHA256加密存储
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SELLER_PASSWORD {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SELLER_PHONE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SELLER_ADDRESS {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short REPUTATION {get;set;}

           /// <summary>
           /// Desc:通过审核的卖家才可以上架商品
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string AUDIT_STATUS {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public decimal CREDIT_NUMBER {get;set;}

    }
}
