﻿using System;
using System.Linq;
using System.Text;

namespace UniTrade.Models
{
    ///<summary>
    ///一个意义不明的表，待确认作用
    ///</summary>
    public partial class MENU
    {
           public MENU(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public decimal ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public decimal PARENTID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ICON {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string MENUNAME {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string URL {get;set;}

    }
}
