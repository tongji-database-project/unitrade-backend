using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace UniTrade.Models
{
    ///<summary>
    ///用户表，有买权限和卖权限
    ///</summary>
    public partial class USERS
    {
           public USERS(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string USER_ID {get;set;}

           /// <summary>
           /// Desc:用户头像，存储图片路径
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AVATAR {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string NAME {get;set;}

           /// <summary>
           /// Desc:加密存储
           /// Default:
           /// Nullable:False
           /// </summary>           
           [JsonIgnore]
           public string PASSWORD {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PHONE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ADDRESS {get;set;}

           /// <summary>
           /// Desc:信誉值
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short REPUTATION {get;set;}

           /// <summary>
           /// Desc:银行卡号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string CREDIT_NUMBER {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SEX {get;set;}

    }
}
