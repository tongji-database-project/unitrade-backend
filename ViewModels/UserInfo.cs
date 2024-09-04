using UniTrade.Models;

namespace UniTrade.ViewModels
{
    public class UserInfo
    {
        public UserInfo(USERS info) {
            id = info.USER_ID;
            avatar = info.AVATAR;
            name = info.NAME;
            phone = info.PHONE;
            address = info.ADDRESS;
            reputation = info.REPUTATION;
            credit_number = info.CREDIT_NUMBER;
            sex = info.SEX;
        }

        public string id { get; set; }
        public string avatar { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public short reputation { get; set; }
        public string credit_number { get; set; }
        public string sex { get; set; }
    }
}
