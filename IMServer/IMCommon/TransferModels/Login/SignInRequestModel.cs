namespace IMCommon.TransferModels
{
    public class SignInRequestModel
    {
        /// <summary>
        /// 用户名，不能重复
        /// </summary>
        public string Username;
        /// <summary>
        /// 密码，明文
        /// </summary>
        public string Password;

        public SignInRequestModel() { }
        public SignInRequestModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
