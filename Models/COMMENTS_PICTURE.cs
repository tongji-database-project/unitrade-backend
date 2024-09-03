using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///评论表的图片
    ///</summary>
    public partial class COMMENTS_PICTURE
    {
           public COMMENTS_PICTURE(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string COMMENT_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PICTURE_PATH {get;set;}

    }
}
