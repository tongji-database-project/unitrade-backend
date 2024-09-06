using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///评论表，用于存放商品下的相关评论
    ///</summary>
    public partial class COMMENTS
    {
           public COMMENTS(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string COMMENT_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string CONTENT {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime COMMENT_TIME {get;set;}

           /// <summary>
           /// Desc:使用一个字符表示该评论是买家收货后发出、卖家回复还是买家追评
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string COMMENT_TYPE {get;set;}

    }
}
