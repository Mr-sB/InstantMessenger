namespace IMCommon
{
    public enum ReturnCode
    {
        Success,
        Fail,
        Exception,
        OperationCodeException,//操作码异常
        SubCodeException,//SubCode异常
        ParameterException,//参数异常
        //Login
        UsernameRepetition,//用户名重复
        UsernameDoesNotExist,//用户名不存在
        PasswordError,//密码错误
        PasswordSame,//新密码与旧密码相同
        //Chat
        ChatMessageCodeException,//聊天码异常
    }
}
