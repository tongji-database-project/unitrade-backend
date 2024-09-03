using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///推送池，用于存放商品特征信息，并根据特征信息进行推送
    ///</summary>
    public partial class RECOMMANDS_POOL
    {
           public RECOMMANDS_POOL(){


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
           public string PREFERENCE {get;set;}

    }
}
