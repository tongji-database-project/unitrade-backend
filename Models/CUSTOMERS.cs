using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///
    ///</summary>
    public partial class CUSTOMERS
    {
           public CUSTOMERS(){


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
           public string CUSTOMER_NAME {get;set;}

           /// <summary>
           /// Desc:使用SHA256加密存储
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string CUSTOMER_PASSWORD {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SEX {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string CUSTOMER_PHONE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string CUSTOMER_ADDRESS {get;set;}

           /// <summary>
           /// Desc:需要验证后才能绑定
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? BANK_NUMBER {get;set;}

    }
}
