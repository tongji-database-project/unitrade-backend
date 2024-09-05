namespace UniTrade.ViewModels
{
    public class LoginInfoViewModel
    {
        public bool UseVerificationCode { get; set; } //是否使用验证码登录
        public string name { get; set; }
        public string password { get; set; }
    }

    public class RegisterInfoViewModel
    {
        public string name { get; set; }
        public string password { get; set; }
        public string PhoneNumber { get; set; } //电话
        public string Email { get; set; } //邮箱
        public string VerificationCode { get; set; }
    }
    public class resetPwdInfoViewModel
    {
        public string name { get; set; }
        public string newPassword { get; set; }
        public string PhoneNumber { get; set; } //电话
        public string Email { get; set; } //邮箱
        public string VerificationCode { get; set; }
    }
    public class CancelInfoViewModel
    {
        public string password { get; set; }
    }
    public class AdminLoginInfoViewModel
    {
        public string name { get; set; }
        public string password { get; set; }
    }
}
