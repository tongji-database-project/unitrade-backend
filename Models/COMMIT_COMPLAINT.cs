using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///关系集，用于存放买家与他发起的投诉
    ///</summary>
    public partial class COMMIT_COMPLAINT
    {
           public COMMIT_COMPLAINT(){


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
           public string COMPLAINT_ID {get;set;}

    }
}
