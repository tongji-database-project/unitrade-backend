using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///商品表，用于存放商品主要信息，商品详情信息存放在其他表中
    ///</summary>
    public partial class MERCHANDISES
    {
           public MERCHANDISES(){


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
           public string MERCHANDISE_NAME {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public decimal PRICE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long INVENTORY {get;set;}

           /// <summary>
           /// Desc:待定
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string MERCHANDISE_TYPE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string COVER_PICTURE_PATH {get;set;}

           /// <summary>
           /// Desc:商品详细信息
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DETAILS {get;set;}

    }
}
