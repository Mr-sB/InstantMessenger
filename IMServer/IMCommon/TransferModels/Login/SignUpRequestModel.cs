namespace IMCommon.TransferModels
{
    public class SignUpRequestModel
    {
        /// <summary>
        /// 用户名，不能重复
        /// </summary>
        public string Username;
        /// <summary>
        /// 密码，明文
        /// </summary>
        public string Password;
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname;

        public SignUpRequestModel() { }
        public SignUpRequestModel(string username, string password, string nickname)
        {
            Username = username;
            Password = password;
            Nickname = nickname;
        }
    }
}
