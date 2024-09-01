namespace UniTrade.ViewModels {
    public class LoginInfoViewModel
    {
        public bool UseVerificationCode { get; set; }  //是否使用验证码登录
        public string name { get; set; }
        public string password { get; set; }
    }
    public class AdminLoginInfoViewModel
    {
        public string name { get; set; }
        public string password { get; set; }
    }
    public class RegisterInfoViewModel
    {
        public string name { get; set; }
        public string password { get; set; }
        //public string confirmpassword { get; set; }
        //public string registerType { get; set; }
        public string Email { get; set; } //邮箱
        public string PhoneNumber { get; set; } //电话
        public string VerificationCode { get; set; }
    }
    public class RefreshTokenViewModel
    {
        public string RefreshToken { get; set; }
    }
    public class LogoutViewModel
    {
        public string RefreshToken { get; set; }
    }
}
