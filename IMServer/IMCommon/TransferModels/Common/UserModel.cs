using IMCommon.DB.Models;

namespace IMCommon.TransferModels
{
    public class UserModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username;
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname;
        public UserModel() { }
        public UserModel(string username, string nickname)
        {
            Init(username, nickname);
        }
        public UserModel(User user)
        {
            if (user == null) return;
            Init(user.Username, user.Nickname);
        }

        private void Init(string username, string nickname)
        {
            Username = username;
            Nickname = nickname;
        }
    }
}
