namespace UniTrade.ViewModels
{
    public class LoginInfoViewModel
    {
        public bool UseVerificationCode { get; set; } //�Ƿ�ʹ����֤���¼
        public string name { get; set; }
        public string password { get; set; }
    }

    public class RegisterInfoViewModel
    {
        public string name { get; set; }
        public string password { get; set; }
        public string PhoneNumber { get; set; } //�绰
        public string Email { get; set; } //����
        public string VerificationCode { get; set; }
    }
    public class resetPwdInfoViewModel
    {
        public string name { get; set; }
        public string newPassword { get; set; }
        public string PhoneNumber { get; set; } //�绰
        public string Email { get; set; } //����
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
