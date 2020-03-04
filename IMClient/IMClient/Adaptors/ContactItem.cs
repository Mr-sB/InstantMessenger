using IMCommon.TransferModels;

namespace IMClient.Adaptors
{
    public class ContactItem
    {
        public readonly string Username;
        public readonly string Nickname;
        public ContactItem(string username, string nickname)
        {
            Username = username;
            Nickname = nickname;
        }

        public ContactItem(UserModel userModel) : this(userModel.Username, userModel.Nickname){}
    }
}