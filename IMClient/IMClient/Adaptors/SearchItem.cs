using IMCommon.TransferModels;

namespace IMClient.Adaptors
{
    public class SearchItem
    {
        public readonly string Username;
        public readonly string Nickname;
        public SearchItem(string username, string nickname)
        {
            Username = username;
            Nickname = nickname;
        }

        public SearchItem(UserModel userModel) : this(userModel.Username, userModel.Nickname){}
    }
}