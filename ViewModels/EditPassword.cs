namespace UniTrade.ViewModels
{
    ///<summary>
    ///修改密码表
    ///</summary>
    public class EditPasswordViewModel
    {
        public EditPasswordViewModel()
        {

        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>  
        public string ORIGIN_PASSWORD { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>  
        public string NEW_PASSWORD { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>  
        public string CONFIRM_PASSWORD { get; set; }
    }
}
