using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///关系集，用于存放指派管理员处理投诉申请的详情信息
    ///</summary>
    public partial class AUDIT_COMPLAINT
    {
           public AUDIT_COMPLAINT(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string COMPLAINT_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ADMIN_ID {get;set;}

    }
}
