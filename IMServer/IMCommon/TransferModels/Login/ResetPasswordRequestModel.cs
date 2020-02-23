namespace IMCommon.TransferModels
{
    public class ResetPasswordRequestModel
    {
        /// <summary>
        /// 旧密码，明文
        /// </summary>
        public string OldPassword;
        /// <summary>
        /// 新密码，明文
        /// </summary>
        public string NewPassword;

        public ResetPasswordRequestModel() { }
        public ResetPasswordRequestModel(string oldPassword, string newPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
}
