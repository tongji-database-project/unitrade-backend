using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///管理员表，与普通用户分开处理
    ///</summary>
    public partial class ADMINISTRATORS
    {
           public ADMINISTRATORS(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ADMIN_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ADMIN_NAME {get;set;}

           /// <summary>
           /// Desc:使用SHA256加密存储
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ADMIN_PASSWORD {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public bool ADMIN_LEVEL {get;set;}

    }
}
